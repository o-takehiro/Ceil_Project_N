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

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize(
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

        // 移動用クラスの生成
        _movement = new PlayerMovement(_rigidbody, _transform, _camera, _animator);
        // 攻撃用クラスの生成
        _attack = new PlayerAttack(_rigidbody, _animator);
        // 攻撃データの初期化
        _attack.SetupAttackData();
        // 魔法用クラスの生成
        _magic = new PlayerMagicAttack(_animator);

    }

    /// <summary>
    /// 使用前準備
    /// </summary>
    public override void Setup(int masterID) {
        base.Setup(masterID);
        // マスター出たー
        var playerMasterID = GetCharacterMaster(masterID);

        // ステータスのセット
        SetMaxHP(playerMasterID.HP);            // 最大HP
        SetHP(playerMasterID.HP);               // HP
        //SetMaxMP(playerMasterID.MP);          // MP
        SetRawAttack(playerMasterID.Attack);    // Attack
        SetRawDefense(playerMasterID.Defense);  // Difence


        // 攻撃データの初期化
        if (_attack != null) {
            _attack.SetupAttackData();
        }

        // カメラターゲット設定
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
        while (!token.IsCancellationRequested) {
            // 次フレームまで待機
            await UniTask.Yield(PlayerLoopTiming.FixedUpdate, token);
            // 移動更新
            _movement.Update(Time.deltaTime, _attack.IsAttacking);

            // 攻撃更新
            await _attack.Update(Time.deltaTime);

            // 座標と回転の更新
            SetPlayerPosition(transform.position);
            SetPlayerRotation(transform.rotation);

        }
    }

    /// <summary>
    /// キャラクター死亡処理
    /// </summary>
    public override void Dead() {
        // 死亡アニメーション再生
    }
}
