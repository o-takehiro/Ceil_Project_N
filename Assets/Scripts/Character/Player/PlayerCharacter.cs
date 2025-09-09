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
    private Transform _transform;       // キャラクターのTransform
    private Camera _camera;             // カメラ参照
    private PlayerInput _playerInput;   // 入力制御
    private Animator _animator;         // アニメーション制御
    private bool _isLockedOn = false;
    public override bool isPlayer() => true;
    public PlayerAttack GetAttackController() => _attack;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void Initialize() {
        // 依存コンポーネントは自前で取得
        _rigidbody = GetComponent<Rigidbody>();
        _animator = GetComponentInChildren<Animator>();
        _playerInput = GetComponent<PlayerInput>();
        _camera = Camera.main; // 必要なら差し替え可

        // 移動用クラスの生成
        _movement = new PlayerMovement(_rigidbody, transform, _camera, _animator);
        // 攻撃用クラスの生成
        _attack = new PlayerAttack(_rigidbody, _animator);
        _attack.SetupAttackData();
        // 魔法用クラスの生成
        _magic = new PlayerMagicAttack(_animator);
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public void InjectDependencies(
        Rigidbody rigidbody,
        Transform transform,
        Camera camera,
        PlayerInput playerInput,
        Animator animator
    ) {
        _rigidbody = rigidbody;
        _transform = transform;
        _camera = camera;
        _playerInput = playerInput;
        _animator = animator;
    }

    /// <summary>
    /// 使用前準備
    /// </summary>
    public override void Setup(int masterID) {
        base.Setup(masterID);

        var playerMasterID = GetCharacterMaster(masterID);
        MenuManager.Instance.Get<PlayerHPGauge>().GetSlider().value = 0.2f;

        SetMaxHP(playerMasterID.HP);
        SetHP(playerMasterID.HP);
        SetRawAttack(playerMasterID.Attack);
        SetRawDefense(playerMasterID.Defense);

        // 座標と回転の更新
        SetPlayerPosition(transform.position);   // MonoBehaviour の transform を使う
        SetPlayerRotation(transform.rotation);
        SetPlayerCenterPosition(transform.position + Vector3.up * 1.5f);
        SetPlayerPrevPosition();
        if (_movement == null) _movement = new PlayerMovement(_rigidbody, transform, _camera, _animator);
        if (_attack == null) _attack = new PlayerAttack(_rigidbody, _animator);
        if (_magic == null) _magic = new PlayerMagicAttack(_animator);


        if (_attack != null) {
            _attack.SetupAttackData();
        }

        if (CameraManager.Instance != null)
            CameraManager.Instance.SetTarget(this);
    }

    // 入力を受けつけて、各クラスで使用可能にする
    public void SetMoveInput(Vector2 input) => _movement.SetMoveInput(input);
    public void RequestJump() => _movement.RequestJump();
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
            
            // 移動の更新処理
            _movement?.MoveUpdate(Time.deltaTime, _attack?.IsAttacking ?? false);

            // 攻撃の更新処理
            if (_attack != null)
                await _attack.Update(Time.deltaTime);

            // 座標と回転の更新
            SetPlayerPosition(transform.position);
            SetPlayerRotation(transform.rotation);
            // 1F前の座標更新
            SetPlayerPrevPosition();
            // 中心座標の更新
            SetPlayerCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));

            // 次フレームまで待機
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
        }
    }

    public float GetPlayerSliderValue() {
        return HP / maxHP;
    }

    /// <summary>
    /// キャラクター死亡処理
    /// </summary>
    public override void Dead() {
        // 死亡アニメーション再生
        _animator.SetTrigger("Death");
        _movement._isDeath = true;
        _magic._isDeath = true;
    }

    public override void Teardown() {
        base.Teardown(); // CharacterBase の TearDown を呼ぶ
        Debug.Log("Teardown");
        _movement?.ResetState();
        _attack?.ResetState();
        _magic?.ResetState();

        // 入力や参照をクリア
        _playerInput = null;

        // Rigidbody の速度をリセット
        if (_rigidbody != null) _rigidbody.velocity = Vector3.zero;

    }


}
