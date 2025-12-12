using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.Windows;
using static CharacterMasterUtility;
using static CharacterUtility;
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
        base.Initialize();

        // コンポーネントの取得
        _rigidbody = GetComponent<Rigidbody>();           // Rigidbody
        _animator = GetComponentInChildren<Animator>();   // アニメーション
        _playerInput = GetComponent<PlayerInput>();       // 入力
        _camera = Camera.main;                            // カメラ

        // 移動制御を生成し、必要な依存を渡す
        if (_movement != null) return;
        _movement = new PlayerMovement(_rigidbody, transform, _camera, _animator, GetComponentInChildren<GroundCheck>());

        // 攻撃制御クラス
        if (_attack != null) return;
        _attack = new PlayerAttack(_rigidbody, _animator, 0);

        // 魔法制御クラス
        if (_magic != null) return;
        _magic = new PlayerMagicAttack();


    }

    /// <summary>
    /// 使用前準備
    /// </summary>
    public override void Setup(int masterID) {
        base.Setup(masterID);

        base.Setup(masterID);


        var master = CharacterMasterUtility.GetCharacterMaster(masterID);
        UnityEngine.Assertions.Assert.IsNotNull(master, "CharacterMaster が取得できません");


        SetMaxHP(master.HP);           // 最大HP
        SetHP(master.HP);              // 現在HP
        SetMaxMP(master.MP);           // 最大MP
        SetMP(master.MP);              // 現在MP
        SetRawAttack(master.Attack);   // 攻撃力
        SetRawDefense(master.Defense); // 防御力

        // 攻撃制御にも反映
        _attack.SetupAttackData();

        // 座標更新
        currentPos = transform.position;
        prevPos = transform.position;
        centerPos = transform.position + Vector3.up * 1.5f;
        currentRot = transform.rotation;

        // 魔法発射位置の登録
        for (int i = 0; i < magicSpawnPoints.Length; i++) {
            if (magicSpawnPoints[i] != null) {
                _magic.SetMagicSpawnPosition(i, magicSpawnPoints[i].gameObject);
            }
        }


        _movement.moveSetUp();


        PausePlayer();


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
    public void RequestJump() {
        _movement.RequestJump();
    }
    /// <summary>
    /// 近接攻撃用入力受付
    /// </summary>
    public void RequestAttack() {
        if (!_movement.IsJumping) {
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

    /// <summary>
    /// 魔法キャスト状態を設定
    /// </summary>
    public void RequestSetCastingFlag(int slotIndex, bool isCasting) {
        _magic.SetCastingFlag(slotIndex, isCasting); // PlayerMagicAttack に状態を渡す
    }

    /// <summary>
    /// キャスト初期の時、エフェクト再生
    /// </summary>
    /// <param name="slotIndex"></param>
    public void RequestStartCasting(int slotIndex) => _magic.StartCasting(slotIndex);

    public void RequestReplaceMagic(int slotIndex) => _magic.ConfirmReplaceMagic(slotIndex);


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
    /// 更新処理
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

    /// <summary>
    /// プレイヤーのメインループ
    /// </summary>
    /// <returns></returns>
    private async UniTaskVoid StartPlayerLoop() {
        if (_isLoopRunning) return;
        _isLoopRunning = true;
        await PlayerMainLoop(this.GetCancellationTokenOnDestroy());
        _isLoopRunning = false;
    }

    /// <summary>
    /// HP減少
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
        _movement.isDeath = true;
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

        // 発動中の魔法を停止
        if (_magic != null) {

        }

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
