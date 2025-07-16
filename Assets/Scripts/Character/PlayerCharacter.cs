/*
 * @file    PlayerCharacter.cs
 * @brief   プレイヤーキャラクター情報
 * @author  Orui
 * @date    2025/7/8
 */
using Cysharp.Threading.Tasks;
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
    private const float _PLAYER_JUMP_SPEED = 7f;
    // 重力
    private const float _PLAYER_GRAVITY = 15f;
    // 落ちる速度
    private const float _FALL_SPEED = 10f;

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
    // キャラクターコントローラー
    private CharacterController _controller;
    // カメラ
    private Camera _camera;
    // PlayerMove.cs
    private PlayerInput _playerMove;

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
    private const float _ATTACK_RESET_TIME = 0.6f; // 秒

    // マスターデータ依存の変数
    public int maxMP { get; private set; } = -1;
    public int MP { get; private set; } = -1;

    public override bool isPlayer() {
        return true;
    }

    [SerializeField] private Rigidbody _rigidbody;  // 追加: Rigidbody参照
    [SerializeField] private LayerMask groundLayer; // 接地判定用レイヤー
    [SerializeField] private float groundCheckDistance = 0.2f; // 地面判定距離
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
    }

    // 外部からの入力受付
    public void SetMoveInput(Vector2 input) => _inputMove = input;
    // ジャンプ受付
    public void RequestJump() => _jumpRequested = true;
    // 攻撃受付
    public void RequestAttack() {
        if (!_isAttacking) {
            _attackRequested = true;
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
            //AttackUpdate(Time.deltaTime);
            // 非同期処理追加版
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

        if (isGrounded && !_wasGrounded) {
            // 接地したら垂直速度をリセット
            _verticalVelocity = 0f;
        }
        else if (!isGrounded) {
            _verticalVelocity -= _PLAYER_GRAVITY * deltaTime;
            if (_verticalVelocity < -_FALL_SPEED) _verticalVelocity = -_FALL_SPEED;
        }

        if (_jumpRequested && isGrounded) {
            _verticalVelocity = _PLAYER_JUMP_SPEED;
        }

        _jumpRequested = false;
        _wasGrounded = isGrounded;

        // 入力からXZ方向の移動を作る
        float camY = _camera.transform.eulerAngles.y;
        Vector3 inputDir = new Vector3(_inputMove.x, 0, _inputMove.y).normalized;
        Vector3 moveDir = Quaternion.Euler(0, camY, 0) * inputDir;

        // 最終的な速度を構成
        Vector3 finalVelocity = moveDir * _PLAYER_RAW_MOVE_SPEED;
        finalVelocity.y = _verticalVelocity;

        // Rigidbodyに直接速度を渡す
        _rigidbody.velocity = finalVelocity;

        // 回転
        if (_inputMove != Vector2.zero) {
            float targetAngle = -Mathf.Atan2(_inputMove.y, _inputMove.x) * Mathf.Rad2Deg + 90f + camY;
            float angleY = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref _turnVelocity, 0.05f);
            _playerMove.ApplyRotation(Quaternion.Euler(0, angleY, 0));
        }
        // 座標更新
        SetPosition(transform.position);
        transform.position = currentPos;
        prevPos = currentPos;
    }

    /// <summary>
    /// 攻撃の非同期処理
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    private async UniTask AttackUpdate(float deltaTime) {
        // 攻撃入力があったら攻撃処理へ
        if (_attackRequested && !_isAttacking) {
            // 攻撃の入力受付をリセット
            _attackRequested = false;
            // 攻撃中に設定
            _isAttacking = true;

            // 3段攻撃を段階的に行う
            AdvanceAttackStep();
            Debug.Log($"攻撃：{_currentAttack}");

            // 攻撃発動時の時間をリセット
            _attackTimer = 0f;

            // 攻撃コライダーを有効にする
            if (attackCollider != null) {
                attackCollider.enabled = true;
            }

            // ---数秒待機---
            await UniTask.Delay(1000);

            // 攻撃コライダーを無効にする
            if (attackCollider != null) {
                attackCollider.enabled = false;
            }
            // 攻撃判定をリセット
            _isAttacking = false;
        }

        // 攻撃ステップが何かに設定されていれば、経過時間を更新
        if (_currentAttack != AttackStep.Invalid) {
            _attackTimer += deltaTime;
            // 攻撃開始からリセット時間分が過ぎたら
            if (_attackTimer >= _ATTACK_RESET_TIME) {
                Debug.Log("攻撃リセット");
                // 攻撃状態を無にする
                _currentAttack = AttackStep.Invalid;
                // 攻撃の経過時間をリセットする
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
