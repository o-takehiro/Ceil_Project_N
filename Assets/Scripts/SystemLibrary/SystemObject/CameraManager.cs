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
    private float rotationSpeed = 3f;                      // 回転速度
    private float mouseSensitivity = 0.05f;                // マウス感度
    private float gamepadSensitivity = 1f;                 // ゲームパッド感度
    private float pitchLimit = 20f;                        // 上下回転の制限

    // キャッシュしたカメラの回転
    private Quaternion _cachedCameraRotation;
    // キャッシュしたカメラの方向
    private Vector3 _cachedCameraDirection;
    // キャッシュしたカメラの座標
    private Vector3 _cachedCameraPosition;
    private bool _wasLockedOnLastFrame = false;
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
        // 追跡用の実態化したプレイヤーを保存
        _target = target.transform;
        playerTarget = _target;
        // 初期角度をプレイヤーに合わせておく
        _currentYaw = _target.eulerAngles.y;
        _currentPitch = 0f;
    }


    /// <summary>
    /// 毎フレーム後に呼ばれる処理（カメラの追従と回転）
    /// </summary>
    private void LateUpdate() {
        if (_camera == null || _target == null) return;

        if (_lockOnSystem.IsLockedOn()) {
            _wasLockedOnLastFrame = true;

            EnemyCharacter enemy = CharacterUtility.GetEnemy();
            if (enemy == null) {
                _lockOnSystem.Unlock();
                return;
            }

            Vector3 playerPos = _target.position;
            Vector3 enemyPos = CharacterUtility.GetEnemyPosition();

            Vector3 directionToEnemy = (enemyPos - playerPos).normalized;

            float distanceBehindPlayer = 5f;
            float heightOffset = 2f;
            Vector3 cameraTargetPosition = playerPos - directionToEnemy * distanceBehindPlayer + Vector3.up * heightOffset;

            float _distance_two = Vector3.Distance(_camera.transform.position, cameraTargetPosition);
            float currentPos = (followSpeed * Time.time) / _distance_two;
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, cameraTargetPosition, currentPos);

            Vector3 lookAtTarget = enemyPos + Vector3.up * 1.5f;
            Quaternion currentRotation = _camera.transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(lookAtTarget - _camera.transform.position);
            float lookSpeed = 5f;
            _camera.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * lookSpeed);

            // 位置と回転をキャッシュ
            _cachedCameraRotation = _camera.transform.rotation;
            _cachedCameraPosition = _camera.transform.position;
        }
        else {
            if (_wasLockedOnLastFrame) {
                _wasLockedOnLastFrame = false;

                // === ロックオン解除直後 ===

                // カメラの位置と回転を完全に復元
                _camera.transform.position = _cachedCameraPosition;
                _camera.transform.rotation = _cachedCameraRotation;

                // カメラの回転からYaw/Pitchを復元（次フレーム以降の回転処理のため）
                Vector3 euler = _cachedCameraRotation.eulerAngles;
                _currentYaw = euler.y;
                _currentPitch = euler.x > 180f ? euler.x - 360f : euler.x;

                // ここで return することで、1フレームは完全に同じ位置・向きのまま処理が終了
                return;
            }

            // === 通常操作 ===

            float sensitivity = Mouse.current != null && Mouse.current.delta.IsActuated()
                ? mouseSensitivity
                : gamepadSensitivity;

            Vector2 delta = _lookInput * sensitivity * rotationSpeed;
            _currentYaw += delta.x;
            _currentPitch = Mathf.Clamp(_currentPitch - delta.y, -pitchLimit, pitchLimit);

            Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
            Vector3 desiredPosition = _target.position + rotation * offset;

            _camera.transform.position = Vector3.Lerp(_camera.transform.position, desiredPosition, followSpeed * Time.deltaTime);
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
