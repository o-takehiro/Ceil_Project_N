using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

using static CharacterUtility;
using static CharacterMasterUtility;
/// <summary>
/// プレイヤーキャラクター全体を統括するクラス
/// ・移動、攻撃などの処理をサブクラスに委譲する
/// ・入力やカメラ制御との橋渡しを行う
/// </summary>
public class PlayerCharacter : CharacterBase {

    private PlayerMovement _movement;   // 移動制御クラス
    private PlayerAttack _attack;       // 攻撃制御クラス
    private PlayerMagicAttack _magic;   // 魔法制御クラス

    private Rigidbody _rigidbody;       // 物理挙動
    private Camera _camera;             // カメラ参照
    private PlayerInput _playerInput;   // 入力制御
    private Animator _animator;         // アニメーション制御
    private bool _isLockedOn = false;
    private bool _isLoopRunning = false;
    private bool _isPaused = false;     // 移動不可
    public override bool isPlayer() => true;
    public PlayerAttack GetAttackController() => _attack;
    public PlayerMovement GetPlayerMovement() => _movement;
    public PlayerMagicAttack GetMagicController() => _magic;
    // 魔法の発射場所
    [SerializeField] private GameObject[] magicSpawnPoints = new GameObject[4];


    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void Initialize() {

    }

    /// <summary>
    /// 使用前準備
    /// </summary>
    public override void Setup(int masterID) {
        base.Setup(masterID);

        // 依存コンポーネントは自前で取得
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _camera = Camera.main; // 必要なら差し替え可

        // プレイヤーをカメラのターゲットに設定する
        if (CameraManager.Instance != null)
            CameraManager.Instance.SetTarget(this);

        // マスターIDを保存
        var playerMasterID = GetCharacterMaster(masterID);
        // プレイヤーのHPゲージを最大にする
        MenuManager.Instance.Get<PlayerHPGauge>().GetSlider().value = 1.0f;

        SetMaxHP(playerMasterID.HP);            // プレイヤーの最大HP設定
        SetHP(playerMasterID.HP);               // プレイヤーのHP設定
        SetMaxMP(playerMasterID.MP);            // プレイヤーの最大MPせてち
        SetMP(playerMasterID.MP);               // プレイヤーのMP接地絵
        SetRawAttack(playerMasterID.Attack);    // プレイーの基礎攻撃力設定
        SetRawDefense(playerMasterID.Defense);  // プレイヤーの基礎防御力設定

        // 座標と回転の更新
        SetPlayerPosition(transform.position);
        SetPlayerRotation(transform.rotation);
        SetPlayerCenterPosition(transform.position + Vector3.up * 1.5f);
        SetPlayerPrevPosition();

        // 接地判定を取得
        var groundCheck = GetComponentInChildren<GroundCheck>();

        if (_movement == null) _movement = new PlayerMovement(_rigidbody, transform, _camera, _animator, groundCheck);
        if (_magic == null) _magic = new PlayerMagicAttack();

        // スロットごとの発射位置を登録
        for (int i = 0; i < magicSpawnPoints.Length; i++) {
            _magic.SetMagicSpawnPosition(i, magicSpawnPoints[i]);

        }

        if (_attack == null) {
            _attack = new PlayerAttack(_rigidbody, _animator, GetRawAttack());
            _attack.SetupAttackData();
        }

        _movement.moveSetUp();
        PausePlayer();
        // メインループを開始する
        StartPlayerLoop().Forget();
    }

    /// <summary>
    /// 移動用入力受付
    /// </summary>
    /// <param name="input"></param>
    public void SetMoveInput(Vector2 input) => _movement.SetMoveInput(input);
    /// <summary>
    /// ジャンプ用入力受付
    /// </summary>
    public void RequestJump() => _movement.RequestJump();
    /// <summary>
    /// 近接攻撃用入力受付
    /// </summary>
    public void RequestAttack() {
        if (_movement._isGrounded) {
            _attack.RequestAttack();
        }
    }
    /// <summary>
    /// 魔法入力の受付x4
    /// </summary>
    /// <param name="slotIndex"></param>
    public void RequestCastMagic(int slotIndex) => _magic.RequestAttack(slotIndex);
    /// <summary>
    /// 魔法のキャンセル入力の受付x4
    /// </summary>
    /// <param name="slotIndex"></param>
    public void RequestCastMagicEnd(int slotIndex) => _magic.RequestCancelMagic(slotIndex);

    public void RequestAnalysis() => _magic.RequestAnalysis();

    /// <summary>
    /// 魔法リストUIの表示
    /// </summary>
    public void RequestOpenMagicUI() => _magic.OpenMagicUI();

    /// <summary>
    /// 魔法リストUIの非表示
    /// </summary>
    public void RequestCloceMagicUI() => _magic.CloseMagicUI();



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
    /// 非同期のUpdate
    /// </summary>
    public async UniTask PlayerMainLoop(CancellationToken token) {
        token = this.GetCancellationTokenOnDestroy();
        while (!token.IsCancellationRequested) {
            // FixdDeltaTimeをキャッシュ
            float fd = Time.fixedDeltaTime;
            if (!_isPaused) {
                // 通常処理
                _movement?.MoveUpdate(fd, _attack?.IsAttacking ?? false);
                _attack?.AttackUpdate(fd);
                _magic?.MagicUpdate();
            }
            else {
                // ポーズ中は強制停止
                if (_rigidbody != null) {
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.angularVelocity = Vector3.zero;
                }
            }
            // 自身のtransform.positoinをキャッシュ
            var pPos = transform.position;
            // 座標の更新
            SetPlayerPosition(pPos);
            SetPlayerPrevPosition();
            // 中心座標の更新
            SetPlayerCenterPosition(new Vector3(pPos.x, pPos.y + 2, pPos.z));
            // 回転の更新
            SetPlayerRotation(transform.rotation);

            // 次フレームまで待機
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
        }
    }

    private async UniTaskVoid StartPlayerLoop() {
        if (_isLoopRunning) return;
        _isLoopRunning = true;
        await PlayerMainLoop(this.GetCancellationTokenOnDestroy());
        _isLoopRunning = false;
    }

    public float GetPlayerSliderValue() {
        _animator.SetTrigger("hit");
        return HP / maxHP;
    }

    public float GetPlayerMPSliderValue() {
        return MP / maxMP;
    }

    /// <summary>
    /// 現在のMP量を返す
    /// </summary>
    /// <returns></returns>
    public float GetPlayerCurrentMP() {
        return MP;
    }

    /// <summary>
    /// キャラクター死亡処理
    /// </summary>
    public override void Dead() {
        // 死亡アニメーション再生
        _animator.SetTrigger("Death");
        _movement._isDeath = true;
        _magic._isDeath = true;
        _magic.ResetMagic();

    }

    /// <summary>
    /// クリアしたときに呼ぶ魔法リセット関数
    /// </summary>
    public void ClearMagicReset() {
        _magic.ResetMagic();
    }


    /// <summary>
    /// プレイヤーの片付け
    /// </summary>
    public override void Teardown() {
        base.Teardown();
        _movement?.ResetState();
        _attack?.ResetState();
        _magic?.ResetState();

        _isLoopRunning = false;

        // Rigidbody の速度をリセット
        if (_rigidbody != null) _rigidbody.velocity = Vector3.zero;
        if (_rigidbody != null) _rigidbody.angularVelocity = Vector3.zero;

        if (CameraManager.Instance != null)
            CameraManager.Instance.UnlockTarget(); // ロック解除

        // カメラのTearDownを呼ぶ
        CameraManager.Instance.TearDown();

    }

    /// <summary>
    /// プレイヤーの行動停止
    /// </summary>
    public void PausePlayer() {
        _isPaused = true;

        // カメラ停止
        //CameraManager.Instance.PauseCamera();

        // 入力停止
        if (_playerInput != null)
            _playerInput.CanReceiveInput = false;

        // アニメーション停止
        if (_animator != null)
            _animator.speed = 0f;


    }

    /// <summary>
    /// プレイヤーの行動再開
    /// </summary>
    public void ResumePlayer() {
        _isPaused = false;

        // 入力再開
        if (_playerInput != null)
            _playerInput.CanReceiveInput = true;

        // アニメーション再開
        if (_animator != null)
            _animator.speed = 1f;
    }

}
