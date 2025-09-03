/*
 * @file    PlayerCharacter.cs
 * @brief   プレイヤーキャラクター情報
 * @author  Orui
 * @date    2025/7/8
 */
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

using static CharacterUtility;
public class PlayerCharacter : CharacterBase {
    //現在のスピード
    public float playerMoveSpeed { get; private set; } = -1.0f;
    [SerializeField] private Collider attackCollider;
    // -定数-
    // 基礎移動スピード
    private const float _PLAYER_RAW_MOVE_SPEED = 10.0f;
    // 基礎ジャンプスピード
    private const float _PLAYER_JUMP_SPEED = 4.7f;
    // 重力
    private const float _PLAYER_GRAVITY = 15f;
    // 落ちる速度
    private const float _FALL_SPEED = 10f;

    [SerializeField] private Rigidbody _rigidbody;  // 追加: Rigidbody参照
    [SerializeField] private LayerMask groundLayer; // 接地判定用レイヤー
    [SerializeField] private float groundCheckDistance = 0.2f; // 地面判定距離
    private bool _isAttackDataInitialized = false;
    // 入力ベクトル
    private Vector2 _inputMove = Vector2.zero;
    // ジャンプ可否
    private bool _jumpRequested = false;
    // 縦方向速度
    private float _verticalVelocity = 0f;
    // Y軸の回転速度補間
    private float _turnVelocity = 0f;
    // 前フレーム接地判定
    private bool _wasGrounded = false;

    // プレイヤーTransgorm
    private Transform _transform;
    // カメラ
    private Camera _camera;
    // PlayerMove.cs
    private PlayerInput _playerMove;

    // 攻撃ごとの設定を保持
    private Dictionary<AttackStep, AttackData> _attackDataMap;

    /// <summary>
    /// プレイヤーの攻撃enum(後でうつすーー)
    /// </summary>
    private enum AttackStep {
        Invalid,    // 無
        First,      // 1段目
        Second,     // 2段目
        Third       // 3段目
    }

    // 現在の攻撃の状態
    private AttackStep _currentAttack = AttackStep.Invalid;
    // 攻撃の入力可否
    private bool _attackRequested = false;
    // 攻撃中かどうか
    private bool _isAttacking = false;
    // 攻撃中の時間
    private float _attackTimer = 0f;
    // 攻撃間のクールタイム
    private const float _ATTACK_RESET_TIME = 5f; // 秒

    // マスターデータ依存の変数
    public int maxMP { get; private set; } = -1;
    public int MP { get; private set; } = -1;
    // 現在ロックオンしているかどうか
    private bool _isLockedOn = false;

    // 魔法を保存するリスト
    //private List<eMagicType> _magicList = null;

    // コンボ攻撃中かどうか
    private bool _comboCheck = false;

    // アニメーター
    [SerializeField] private Animator animator;
    private const string _ANIMATION_RUN = "Run";
    private const string _ANIMATION_RUN_STOP = "Run_Stop";
    private const string _ANIMATION_IDLE = "Idle";

    public override bool isPlayer() {
        return true;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="transform"></param>
    /// <param name="camera"></param>
    /// <param name="engineAdapter"></param>
    public void Initialize(
        Rigidbody rigidbody,
        Transform transform,
        Camera camera,
        PlayerInput engineAdapter) {
        _rigidbody = rigidbody;
        _transform = transform;
        _camera = camera;
        _playerMove = engineAdapter;
    }

    /// <summary>
    /// 使用前準備
    /// </summary>
    public override void Setup() {
        base.Setup();
        // カメラに自身をセット
        if (CameraManager.Instance != null) CameraManager.Instance.SetTarget(GetPlayer());
        if (attackCollider != null) {
            attackCollider.enabled = false;
        }
        SetupAttackData(); // 攻撃データ初期化
        // 座標更新
        SetPlayerPosition(transform.position);
        transform.position = currentPos;

        // 回転更新
        SetPlayerRotation(transform.rotation);

        // フラグを初期化
        _comboCheck = false;
    }

    // 外部からの入力受付
    public void SetMoveInput(Vector2 input) => _inputMove = input;
    // ジャンプ受付
    public void RequestJump() {
        _jumpRequested = true;
    }
    // 攻撃受付
    public void RequestAttack() {
        if (!_isAttacking) {
            _attackRequested = true;
        }
    }
    // カメラのロックオン受付
    public void RequestLookOn() {
        if (_isLockedOn) {
            // ロックオン解除
            CameraManager.Instance.UnlockTarget();
            _isLockedOn = false;
        }
        else {
            // 敵が取得できる場合のみロックオン
            var enemy = CharacterUtility.GetEnemy();
            if (enemy != null) {
                CameraManager.Instance.LockOnTarget(enemy);
                _isLockedOn = true;
            }
        }
    }

    /// <summary>
    /// 非同期処理 ; Update
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTask PlayerMainLoop(CancellationToken token) {
        // 無限ループ
        while (!token.IsCancellationRequested) {
            // 移動
            MoveUpdate(Time.deltaTime);
            // 攻撃
            await AttackUpdate(Time.deltaTime);

            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }


    /// <summary>
    /// 移動関連の1フレーム分の更新
    /// </summary>
    private void MoveUpdate(float deltaTime) {
        if (_isAttacking) return;

        bool isGrounded = CheckGrounded();

        // ジャンプ入力処理
        if (_jumpRequested && isGrounded) {
            _rigidbody.AddForce(Vector3.up * _PLAYER_JUMP_SPEED, ForceMode.VelocityChange);
            // アニメーションを再生
            animator.SetTrigger("jumpT");
        }

        // 着地を検知
        if (isGrounded && !_wasGrounded) {
            animator.SetBool(_ANIMATION_IDLE, true);
        }

        _jumpRequested = false;
        _wasGrounded = isGrounded;

        // 入力ベクトルからワールド移動方向を計算（カメラ基準）
        Vector3 inputDir = new Vector3(_inputMove.x, 0f, _inputMove.y).normalized;
        Vector3 moveDir = Quaternion.Euler(0f, _camera.transform.eulerAngles.y, 0f) * inputDir;

        // Rigidbody の現在の y 速度はそのまま維持する
        Vector3 finalVelocity = moveDir * _PLAYER_RAW_MOVE_SPEED;
        finalVelocity.y = _rigidbody.velocity.y;
        _rigidbody.velocity = finalVelocity;

        // 入力がある場合はモデルの向きを移動方向に合わせる
        if (inputDir.sqrMagnitude > 0.001f) {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float angleY = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref _turnVelocity, 0.1f);
            _transform.rotation = Quaternion.Euler(0f, angleY, 0f);
        }

        // 座標更新
        SetPlayerPosition(_transform.position);
        _transform.position = currentPos;
        prevPos = currentPos;

        // 回転更新
        SetPlayerRotation(_transform.rotation);

        // アニメーション制御
        bool hasInput = _inputMove != Vector2.zero;
        if (hasInput) {
            animator.SetBool(_ANIMATION_RUN, true);
            animator.SetBool(_ANIMATION_RUN_STOP, false);
        }
        else {
            if (animator.GetBool(_ANIMATION_RUN)) {
                animator.SetBool(_ANIMATION_RUN, false);
                animator.SetBool(_ANIMATION_RUN_STOP, true);
            }
        }
    }
    // Idleアニメーション
    public void OnStopAnimationEnd() {
        animator.SetBool(_ANIMATION_RUN_STOP, false);
    }
    /// <summary>
    /// 攻撃の非同期処理
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async UniTask AttackUpdate(float deltaTime) {
        if (_attackRequested && !_isAttacking) {
            _attackRequested = false;
            _isAttacking = true;
            _comboCheck = true;
            if (_comboCheck) {

                // 攻撃開始時に移動を止める
                _rigidbody.velocity = Vector3.zero;
            }

            AdvanceAttackStep();
            Debug.Log($"攻撃ステップ: {_currentAttack}");

            // 攻撃データ取得
            if (!_attackDataMap.TryGetValue(_currentAttack, out var attackData)) {
                _isAttacking = false;
                return;
            }

            // アニメーション再生
            PlayAttackAnimation(attackData.AnimationName);

            // コライダーON時間分待機
            await UniTask.Delay(attackData.ColliderActiveDurationMs);

            // 硬直ディレイ
            await UniTask.Delay(attackData.PostDelayMs);

            _isAttacking = false;
            _comboCheck = false;
        }

        // 攻撃リセット判定
        if (_currentAttack != AttackStep.Invalid) {
            _attackTimer += deltaTime;
            if (_attackTimer >= _ATTACK_RESET_TIME) {
                Debug.Log("攻撃リセット");
                _currentAttack = AttackStep.Invalid;
                _attackTimer = 0f;
            }
        }

    }

    /// <summary>
    /// 現在の攻撃段階を次に進める。
    /// </summary>
    private void AdvanceAttackStep() {
        // 現在の攻撃からの遷移
        switch (_currentAttack) {
            case AttackStep.Invalid:
                _currentAttack = AttackStep.First;
                break;
            case AttackStep.First:  // 1段目から2段目
                _currentAttack = AttackStep.Second;
                break;
            case AttackStep.Second: // 2段目から3段目
                _currentAttack = AttackStep.Third;
                break;
            default:                // 1段目に戻す
                _currentAttack = AttackStep.First;
                break;
        }
    }


    /// <summary>
    /// 攻撃事のデータの初期化
    /// </summary>
    private void SetupAttackData() {
        if (_isAttackDataInitialized) return;

        _attackDataMap = new Dictionary<AttackStep, AttackData> {
        {
            AttackStep.First,
            new AttackData("attack", 10f, 500, 0)
        },
        {
            AttackStep.Second,
            new AttackData("attack", 15f, 500, 0)
        },
        {
            AttackStep.Third,
            new AttackData("attack", 20f, 1000, 3)
        }
    };

        _isAttackDataInitialized = true;

    }

    /// <summary>
    /// アニメーションの再生
    /// </summary>
    /// <param name="animationName"></param>
    private void PlayAttackAnimation(string animationName) {
        animator.SetTrigger(animationName);
        _comboCheck = true;
    }

    /// <summary>
    /// 地面の接地判定
    /// </summary>
    /// <returns></returns>
    private bool CheckGrounded() {
        Vector3 origin = _transform.position + Vector3.up * 0.1f;
        return Physics.Raycast(origin, Vector3.down, groundCheckDistance + 0.1f, groundLayer);
    }


    /// <summary>
    /// キャラクターの死亡
    /// </summary>
    public override void Dead() {

    }
}
