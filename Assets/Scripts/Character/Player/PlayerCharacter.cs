using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

/// <summary>
/// プレイヤーキャラクター全体を統括するクラス
/// ・移動、攻撃などの処理をサブクラスに委譲する
/// ・入力やカメラ制御との橋渡しを行う
/// </summary>
public class PlayerCharacter : CharacterBase {
    private PlayerMovement _movement;   // 移動制御クラス
    private PlayerAttack _attack;       // 攻撃制御クラス

    private Rigidbody _rigidbody;       // 物理挙動
    private Transform _transform;       // キャラクターのTransform
    private Camera _camera;             // カメラ参照
    private PlayerInput _playerInput;   // 入力制御
    private Animator _animator;         // アニメーション制御
    private bool _isLockedOn = false;
    public override bool isPlayer() => true;

    /// <summary>
    /// 外部から初期化されるコンストラクタ的処理
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

        // サブクラスを生成
        _movement = new PlayerMovement(rigidbody, transform, camera, animator);
        _attack = new PlayerAttack(rigidbody, animator);
        _attack.SetupAttackData();
    }

    /// <summary>
    /// ゲーム開始時の初期化処理
    /// </summary>
    public override void Setup() {
        base.Setup();

        // 攻撃データの初期化は Setup 内で呼ぶ
        if (_attack != null) {
            _attack.SetupAttackData();
            Debug.Log("攻撃データの初期化完了");
        }

        // カメラターゲット設定
        if (CameraManager.Instance != null)
            CameraManager.Instance.SetTarget(this);
    }

    // 入力受付を移動・攻撃クラスに転送
    public void SetMoveInput(Vector2 input) => _movement.SetMoveInput(input);
    public void RequestJump() => _movement.RequestJump();
    public void RequestAttack() => _attack.RequestAttack();
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
    /// 非同期でプレイヤーの毎フレーム処理を実行するループ
    /// </summary>
    public async UniTask PlayerMainLoop(CancellationToken token) {
        while (!token.IsCancellationRequested) {
            // 移動更新（攻撃中は制御される）
            _movement.Update(Time.deltaTime, _attack.IsAttacking);

            // 攻撃更新（非同期処理あり）
            await _attack.Update(Time.deltaTime);

            // 次フレームまで待機
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }

    /// <summary>
    /// キャラクター死亡処理
    /// </summary>
    public override void Dead() {
        // TODO: 死亡アニメーションやリスポーン処理
    }
}
