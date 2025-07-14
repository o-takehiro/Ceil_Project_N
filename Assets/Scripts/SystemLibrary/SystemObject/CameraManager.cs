using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : SystemObject {
    // Singletonインスタンス
    public static CameraManager Instance { get; private set; } = null;

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
    private Transform _target;                // 実際のターゲット参照

    // カメラの挙動に関する設定
    public Vector3 offset = new Vector3(0f, 2f, -5f);   // プレイヤーに対するカメラの相対位置
    public float followSpeed = 10f;                     // カメラの追従速度
    public float rotationSpeed = 1.0f;                  // 回転速度
    public float mouseSensitivity = 0.1f;               // マウス感度
    public float gamepadSensitivity = 1.0f;             // ゲームパッド感度
    public float pitchLimit = 40f;                      // 上下回転の制限

    /// <summary>
    /// 初期化処理（非同期）
    /// </summary>
    public override async UniTask Initialize() {
        Instance = this;

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
    public void SetTarget(Transform target) {
        this.playerTarget = target;
    }

    /// <summary>
    /// 毎フレーム後に呼ばれる処理（カメラの追従と回転）
    /// </summary>
    private void LateUpdate() {
        if (_camera == null || _target == null) return;

        // 入力デバイスごとの感度を選択
        float sensitivity = Mouse.current != null && Mouse.current.delta.IsActuated()
            ? mouseSensitivity
            : gamepadSensitivity;

        // 入力から回転を更新
        Vector2 delta = _lookInput * sensitivity * rotationSpeed;
        _currentYaw += delta.x;
        _currentPitch = Mathf.Clamp(_currentPitch - delta.y, -pitchLimit, pitchLimit);

        // カメラの回転をQuaternionで計算
        Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);

        // プレイヤー位置から回転を加味したカメラの理想位置を算出
        Vector3 desiredPosition = _target.position + rotation * offset;

        // カメラをスムーズに追従させる
        _camera.transform.position = Vector3.Lerp(_camera.transform.position, desiredPosition, followSpeed * Time.deltaTime);

        // プレイヤーを注視（頭あたりを見るようにオフセットを加える）
        _camera.transform.LookAt(_target.position + Vector3.up * 1.5f);
    }

    /// <summary>
    /// オブジェクト破棄時の処理（入力を解除）
    /// </summary>
    private void OnDestroy() {
        _inputActions?.Disable();
    }

}
