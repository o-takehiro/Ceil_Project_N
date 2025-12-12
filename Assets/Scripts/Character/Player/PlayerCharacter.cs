/*
 *  @file   PlayerCharacter
 *  @author oorui
 */

using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

using static CharacterUtility;
using static CharacterMasterUtility;

/// <summary>
/// プレイヤークラス
/// </summary>
public class PlayerCharacter : CharacterBase {

    private PlayerMovement _movement;       // 移動制御
    private PlayerAttack _attack;           // 攻撃制御
    private PlayerMagicAttack _magic;       // 魔法制御

    private Rigidbody _rigidbody;           // Rigidbody
    private Camera _camera;                 // カメラ
    private PlayerInput _playerInput;       // 入力
    private Animator _animator;             // アニメーション

    private bool _isLockedOn = false;       // ロックオン使用可否
    private bool _isLoopRunning = false;    // メインループ可否
    private bool _isPaused = false;         // メインループ

    // 魔法の発射位置
    [SerializeField] private GameObject[] magicSpawnPoints = new GameObject[4];

    public override bool isPlayer() => true;


    /// <summary>
    /// 初期化
    /// </summary>
    public override void Initialize() {
        base.Initialize();
    }

    /// <summary>
    /// 使用前準備
    /// </summary>
    public override void Setup(int masterID) {
        base.Setup(masterID);

        // コンポーネントの取得
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _camera = Camera.main;

        // カメラのターゲットをプレイヤーに設定
        if (CameraManager.Instance != null)
            CameraManager.Instance.SetTarget(this);

        // プレイヤーのマスターIDの設定
        var masterData = GetCharacterMaster(masterID);

        // プレイヤーのステータスの設定
        MenuManager.Instance.Get<PlayerHPGauge>().GetSlider().value = 1.0f;

        SetMaxHP(masterData.HP);
        SetHP(masterData.HP);
        SetMaxMP(masterData.MP);
        SetMP(masterData.MP);
        SetRawAttack(masterData.Attack);
        SetRawDefense(masterData.Defense);

        // 座標、回転の初期化
        UpdatePlayerTransformCache();

        // 地面接地判定の取得
        var groundCheck = GetComponentInChildren<GroundCheck>();

        // それぞれのコンポーネントを生成
        // 移動
        if (_movement == null)
            _movement = new PlayerMovement(_rigidbody, transform, _camera, _animator, groundCheck);
        // 魔法
        if (_magic == null)
            _magic = new PlayerMagicAttack();
        // 近接攻撃
        if (_attack == null) {
            _attack = new PlayerAttack(_rigidbody, _animator, GetRawAttack());
            _attack.SetupAttackData();
        }

        // 魔法の発射位置を設定
        for (int i = 0; i < magicSpawnPoints.Length; i++) {
            _magic.SetMagicSpawnPosition(i, magicSpawnPoints[i]);
        }

        // 移動可能にする
        _movement.moveSetUp();

        // SetUp中はプレイヤーの行動を止める
        PausePlayer();

        // メインループ開始
        StartPlayerLoop().Forget();
    }


    public void SetMoveInput(Vector2 input) => _movement.SetMoveInput(input);

    public void RequestJump() => _movement.RequestJump();

    public void RequestAttack() {
        if (!_movement.IsJumping)
            _attack.RequestAttack();
    }

    public void RequestCastMagic(int slotIndex) => _magic.RequestAttack(slotIndex);

    public void RequestCastMagicEnd(int slotIndex) => _magic.RequestCancelMagic(slotIndex);

    public void RequestSetCastingFlag(int index, bool isCasting) => _magic.SetCastingFlag(index, isCasting);

    public void RequestStartCasting(int slotIndex) => _magic.StartCasting(slotIndex);

    public void RequestReplaceMagic(int slotIndex) => _magic.ConfirmReplaceMagic(slotIndex);

    public void RequestAnalysis() => _magic.RequestAnalysis();

    public void RequestOpenMagicUI() => _magic.OpenMagicUI();

    public void RequestCloceMagicUI() => _magic.CloseMagicUI();

    /// <summary>
    /// ロックオン切り替え
    /// </summary>
    public void RequestLookOn() {
        if (_isLockedOn) {
            CameraManager.Instance.UnlockTarget();
            _isLockedOn = false;
            return;
        }
        // 敵の取得
        var enemy = CharacterUtility.GetEnemy();
        if (enemy != null) {
            // 敵の方を見る
            CameraManager.Instance.LockOnTarget(enemy);
            _isLockedOn = true;
        }
    }

    /// <summary>
    /// プレイヤーのループを開始
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid StartPlayerLoop() {
        if (_isLoopRunning) return;
        _isLoopRunning = true;

        // トークんを渡してメインループじっこう　
        await PlayerMainLoop(this.GetCancellationTokenOnDestroy());

        _isLoopRunning = false;
    }

    /// <summary>
    /// メインループ
    /// </summary>
    public async UniTask PlayerMainLoop(CancellationToken token) {
        // トークンを取得
        token = this.GetCancellationTokenOnDestroy();

        // 無限ループ
        while (!token.IsCancellationRequested) {
            // 時間を取得
            float fd = Time.fixedDeltaTime;

            if (!_isPaused) {
                // それぞれの更新処理
                _movement?.MoveUpdate(fd, _attack?.IsAttacking ?? false);   // 移動
                _attack?.AttackUpdate(fd);                                  // 攻撃
                _magic?.MagicUpdate();                                      // 魔法
            }
            else {
                // 移動停止
                ResetRigidbodyVelocity();
            }

            // 座標、回転の更新
            UpdatePlayerTransformCache();

            // 次のフレームまで待機
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
        }
    }

    /// <summary>
    /// Rigidbody の速度をなくす
    /// </summary>
    private void ResetRigidbodyVelocity() {
        if (_rigidbody == null) return;

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;
    }

    /// <summary>
    /// 座標などの更新
    /// </summary>
    private void UpdatePlayerTransformCache() {
        Vector3 pos = transform.position;

        SetPlayerPosition(pos);
        SetPlayerPrevPosition();
        SetPlayerCenterPosition(pos + Vector3.up * 2f);
        SetPlayerRotation(transform.rotation);
    }

    /// <summary>
    /// 被弾処理
    /// </summary>
    /// <returns></returns>
    public float GetPlayerSliderValue() {
        _animator.SetTrigger("hit");
        return HP / maxHP;
    }

    /// <summary>
    /// MP減少
    /// </summary>
    /// <returns></returns>
    public float GetPlayerMPSliderValue() {
        return MP / maxMP;
    }

    /// <summary>
    /// プレイヤーのMP取得
    /// </summary>
    /// <returns></returns>
    public float GetPlayerCurrentMP() { return MP; }

    /// <summary>
    /// 死亡処理
    /// </summary>
    public override void Dead() {
        _animator.SetTrigger("Death");

        _movement.isDeath = true;
        _magic._isDeath = true;

        _magic.ResetMagic();
    }

    /// <summary>
    /// 魔法の片付け
    /// </summary>
    public void ClearMagicReset() { _magic.ResetMagic(); }

    /// <summary>
    /// 片付け処理
    /// </summary>
    public override void Teardown() {
        base.Teardown();

        // 各コンポーネントの状態を初期化
        _movement?.ResetState();
        _attack?.ResetState();
        _magic?.ResetState();

        _isLoopRunning = false;

        ResetRigidbodyVelocity();

        if (CameraManager.Instance != null) {
            CameraManager.Instance.UnlockTarget();
            CameraManager.Instance.TearDown();
        }
    }

    /// <summary>
    /// プレイヤーの動きを停止
    /// </summary>
    public void PausePlayer() {
        _isPaused = true;

        if (_playerInput != null)
            _playerInput.CanReceiveInput = false;

        if (_animator != null)
            _animator.speed = 0f;
    }

    /// <summary>
    /// プレイヤーの動きを再開
    /// </summary>
    public void ResumePlayer() {
        _isPaused = false;

        if (_playerInput != null)
            _playerInput.CanReceiveInput = true;

        if (_animator != null)
            _animator.speed = 1f;
    }

    /// <summary>
    /// プレイヤーの攻撃クラスの取得
    /// </summary>
    /// <returns></returns>
    public PlayerAttack GetAttackController() { return _attack; }
    /// <summary>
    /// プレイヤーの移動クラス取得
    /// </summary>
    /// <returns></returns>
    public PlayerMovement GetPlayerMovement() { return _movement; }
    /// <summary>
    /// プレイヤーの魔法クラス取得
    /// </summary>
    /// <returns></returns>
    public PlayerMagicAttack GetMagicController() { return _magic; }
}
