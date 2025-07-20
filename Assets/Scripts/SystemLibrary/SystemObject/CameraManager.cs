using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class CameraManager : SystemObject {
    // Singletonインスタンス
    public static CameraManager Instance { get; private set; } = null;
    private LockOnSystem _lockOnSystem = new LockOnSystem();
    // メインカメラの参照
    private Camera _camera;
    private const string _CAMERA_NAME = "Main Camera";

    // 入力アクション
    private CameraInputActions _inputActions;

    // カメラ操作用の入力ベクトル
    private Vector2 _lookInput;

    // カメラの回転制御
    private float _currentYaw;
    private float _currentPitch;

    public Transform playerTarget;           // 追従対象となるプレイヤー
    private Transform _target;               // 実際のターゲット参照

    // カメラの挙動に関する設定
    private Vector3 offset = new Vector3(0f, 2f, -5f);     // プレイヤーに対するカメラの相対位置
    private float followSpeed = 10f;                       // カメラの追従速度
    private float rotationSpeed = 4f;                      // 回転速度
    private float mouseSensitivity = 0.05f;                // マウス感度
    private float gamepadSensitivity = 0.1f;               // ゲームパッド感度
    private float pitchLimit = 20f;                        // 上下回転の制限

    /// <summary>
    /// 初期化処理（非同期）
    /// </summary>
    public override async UniTask Initialize() {
        Instance = this;

        await UniTask.DelayFrame(1); // プレイヤー生成待ち
        // シーン上からMain Cameraを取得
        _camera = GameObject.Find(_CAMERA_NAME).GetComponent<Camera>();

        // 入力アクションの初期化
        _inputActions = new CameraInputActions();

        // Look入力（マウス・右スティック）を監視
        _inputActions.Camera.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>();
        _inputActions.Camera.Look.canceled += ctx => _lookInput = Vector2.zero;
        _inputActions.Enable();

        // プレイヤーのTransformを取得（Inspectorで指定されていなければタグから検索）
        if (playerTarget != null) {
            _target = playerTarget;
        }
        else {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) _target = player.transform;
        }

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 追跡対象を設定
    /// </summary>
    /// <param name="target"></param>
    public void SetTarget(PlayerCharacter target) {
        _target = target.transform;      // 追跡用の実体も更新
        playerTarget = _target;
        // 初期角度をプレイヤーに合わせてリセットしておくと視点が飛びにくい
        _currentYaw = _target.eulerAngles.y;
        _currentPitch = 0f;
    }


    /// <summary>
    /// 毎フレーム後に呼ばれる処理（カメラの追従と回転）
    /// </summary>
    private void LateUpdate() {
        if (_camera == null || _target == null) return;

        // ロックオン時の挙動
        if (_lockOnSystem.IsLockedOn()) {

            // 敵キャラクターの取得
            EnemyCharacter enemy = CharacterUtility.GetEnemy();
            if (enemy == null) {
                // 敵が消滅または取得失敗したらロックオン解除
                _lockOnSystem.Unlock();
                return;
            }

            // プレイヤーと敵のワールド座標を取得
            Vector3 playerPos = _target.position;
            Vector3 enemyPos = CharacterUtility.GetEnemyPosition();

            // プレイヤーと敵の中心点（注視ポイントとして使用可能）
            Vector3 midpoint = (playerPos + enemyPos) * 0.5f;

            // プレイヤー→敵の方向を取得（正規化で方向ベクトル化）
            Vector3 directionToEnemy = (enemyPos - playerPos).normalized;

            // カメラの理想的な位置を算出（敵とプレイヤーを両方映すような距離・高さ）
            float distanceBehindPlayer = 5f;   // 後ろに引く距離
            float heightOffset = 2f;           // 高さ（Y軸）オフセット
            Vector3 cameraTargetPosition = playerPos - directionToEnemy * distanceBehindPlayer + Vector3.up * heightOffset;

            // カメラをスムーズに目的位置へ移動させる
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, cameraTargetPosition, followSpeed * Time.deltaTime);

            // 敵の頭部（少し上）をカメラが注視するように設定
            _camera.transform.LookAt(enemyPos + Vector3.up * 1.5f);
        }
        // 通常のカメラ挙動
        else {

            // 使用デバイスごとに感度を切り替える
            float sensitivity = Mouse.current != null && Mouse.current.delta.IsActuated()
                ? mouseSensitivity
                : gamepadSensitivity;

            // 入力ベクトルから回転を計算
            Vector2 delta = _lookInput * sensitivity * rotationSpeed;
            _currentYaw += delta.x;
            _currentPitch = Mathf.Clamp(_currentPitch - delta.y, -pitchLimit, pitchLimit);

            // カメラの回転角度をQuaternionに変換
            Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);

            // プレイヤー基準のカメラ位置を取得
            Vector3 desiredPosition = _target.position + rotation * offset;

            // カメラを補間でスムーズに移動させる
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // プレイヤーのやや上をカメラが常に注視する
            _camera.transform.LookAt(_target.position + Vector3.up * 1.5f);
        }
    }

    // ロックオン対象を設定するAPI
    public void LockOnTarget(EnemyCharacter target) {
        if (target == null) return;

        // Transform を渡す
        _lockOnSystem.LockOn(target.transform);
    }

    // ロックオン解除
    public void UnlockTarget() {
        _lockOnSystem.Unlock();
    }

    /// <summary>
    /// オブジェクト破棄時の処理（入力を解除）
    /// </summary>
    private void OnDestroy() {
        _inputActions?.Disable();
    }

}
