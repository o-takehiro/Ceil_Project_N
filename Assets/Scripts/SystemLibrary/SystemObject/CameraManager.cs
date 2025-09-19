using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// カメラの挙動を制御するクラス
/// </summary>
public class CameraManager : SystemObject {
    public static CameraManager Instance { get; private set; } = null;
    private PlayerMovement _movement;   // 移動制御クラス
    // 敵を記録してロックオン管理する仕組み
    private LockOnSystem _lockOnSystem = new LockOnSystem();

    // 実際に動かす対象となるカメラ
    private Camera _camera;
    private const string _CAMERA_NAME = "Main Camera";

    // 入力（マウスやゲームパッド）をまとめたクラス
    private CameraInputActions _inputActions;

    // 視点入力の値（横方向・縦方向）
    private Vector2 _lookInput;

    // カメラの回転角度を記録するための変数
    private float _currentYaw;   // 横回転
    private float _currentPitch; // 縦回転

    // 追従対象（プレイヤーなど）
    public Transform playerTarget;
    private Transform _target;

    // カメラの動きに関する設定値
    private Vector3 offset = new Vector3(0f, 4f, -8f); // プレイヤーからどの位置に置くか
    private float followSpeed = 10f;                   // プレイヤーを追いかける速さ
    private float rotationSpeed = 3f;                  // 視点操作の速さ
    private float mouseSensitivity = 0.05f;            // マウスで操作するときの感度
    private float gamepadSensitivity = 1f;             // ゲームパッドで操作するときの感度
    private float pitchLimit = 20f;                    // 上下の視点移動の限界値

    // 前回のカメラ状態を記録しておくための変数
    private Quaternion _cachedCameraRotation;
    private Vector3 _cachedCameraDirection; // 今は使っていないが将来のために残している
    private Vector3 _cachedCameraPosition;
    private bool _wasLockedOnLastFrame = false; // 前フレームでロックオンしていたかどうか

    // カメラ演出用
    private bool _isFocusing = false;   // 演出中フラグ
    private CancellationTokenSource _focusCts;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override async UniTask Initialize() {
        Instance = this;
        CameraSetUp();
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 準備
    /// </summary>
    public void CameraSetUp() {

        // シーン内からメインカメラを探す
        _camera = GameObject.Find(_CAMERA_NAME).GetComponent<Camera>();

        // 入力設定を初期化
        _inputActions = new CameraInputActions();
        _inputActions.Camera.Look.performed += ctx => _lookInput = ctx.ReadValue<Vector2>(); // 入力中の値を受け取る
        _inputActions.Camera.Look.canceled += ctx => _lookInput = Vector2.zero;              // 入力をやめたらリセット
        _inputActions.Enable();

        // プレイヤーの参照を取得
        if (playerTarget != null) {
            _target = playerTarget;
        }
        else {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null) _target = player.transform;
        }
    }

    /// <summary>
    /// プレイヤーを追従対象に設定する
    /// </summary>
    public void SetTarget(PlayerCharacter target) {
        _target = target.transform;
        playerTarget = _target;

        // プレイヤーの正面を向く
        _camera.transform.rotation = Quaternion.LookRotation(_target.forward, Vector3.up);


        // プレイヤーの向きは無視してカメラのYawを維持
        // _currentYaw はカメラの現状を保持する
        Vector3 camEuler = _camera.transform.rotation.eulerAngles;
        _currentYaw = camEuler.y;
        _currentPitch = camEuler.x > 180f ? camEuler.x - 360f : camEuler.x;

        // キャッシュもリセット
        _cachedCameraRotation = _camera.transform.rotation;
        _cachedCameraPosition = _camera.transform.position;
    }

    /// <summary>
    /// 毎フレームのカメラ制御
    /// - プレイヤー追従
    /// - 入力による回転
    /// - ロックオン時の処理
    /// </summary>
    private void LateUpdate() {
        if (_camera == null || _target == null) return;
        if (_isFocusing) return;
        // --- ロックオン中 ---
        if (_lockOnSystem.IsLockedOn()) {
            _wasLockedOnLastFrame = true;

            // 現在の敵を取得
            EnemyCharacter enemy = CharacterUtility.GetEnemy();
            if (enemy == null) {
                _lockOnSystem.Unlock();
                return;
            }

            // プレイヤーと敵の位置を取得
            Vector3 playerPos = _target.position;
            Vector3 enemyPos = CharacterUtility.GetEnemyPosition();

            // プレイヤーから敵への方向
            Vector3 directionToEnemy = (enemyPos - playerPos).normalized;

            // カメラをプレイヤーの後方＋高さに配置
            float distanceBehindPlayer = 5f;
            float heightOffset = 3.5f;
            Vector3 cameraTargetPosition = playerPos - directionToEnemy * distanceBehindPlayer + Vector3.up * heightOffset;

            // 補間を使ってカメラをスムーズに移動
            float _distance_two = Vector3.Distance(_camera.transform.position, cameraTargetPosition);
            float currentPos = (followSpeed * Time.time) / _distance_two;
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, cameraTargetPosition, currentPos);

            // 敵の方向を向くように回転を補間
            Vector3 lookAtTarget = enemyPos + Vector3.up * 1.5f; // 敵の少し上（頭の高さ）
            Quaternion currentRotation = _camera.transform.rotation;
            Quaternion targetRotation = Quaternion.LookRotation(lookAtTarget - _camera.transform.position);
            float lookSpeed = 5f;
            _camera.transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, Time.deltaTime * lookSpeed);

            // 現在の位置と回転をキャッシュしておく
            _cachedCameraRotation = _camera.transform.rotation;
            _cachedCameraPosition = _camera.transform.position;
        }
        // 非ロックオン状態
        else {
            if (_wasLockedOnLastFrame) {
                _wasLockedOnLastFrame = false;

                // ロックオン解除後、元の状態に戻す
                _camera.transform.position = _cachedCameraPosition;     // 座標
                _camera.transform.rotation = _cachedCameraRotation;     // 回転

                // カメラの回転角度を戻す
                Vector3 euler = _cachedCameraRotation.eulerAngles;
                _currentYaw = euler.y;
                _currentPitch = euler.x > 180f ? euler.x - 360f : euler.x;

                return;
            }

            // 入力デバイスに応じて感度を切り替える
            float sensitivity = Mouse.current != null && Mouse.current.delta.IsActuated()
                ? mouseSensitivity      // マウス
                : gamepadSensitivity;   // パッド

            // カメラの回転を更新
            Vector2 delta = _lookInput * sensitivity * rotationSpeed;
            _currentYaw += delta.x;
            _currentPitch = Mathf.Clamp(_currentPitch - delta.y, -pitchLimit, pitchLimit);

            // カメラの位置を求める
            Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
            Vector3 desiredPosition = _target.position + rotation * offset;

            // 補間でスムーズに移動
            _camera.transform.position = Vector3.Lerp(_camera.transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // プレイヤーの頭上を注視
            _camera.transform.LookAt(_target.position + Vector3.up * 1.5f);
        }
    }

    /// <summary>
    /// 敵をロックオン対象として設定する
    /// - 敵の Transform を渡してロックオン状態にする
    /// </summary>
    public void LockOnTarget(EnemyCharacter target) {
        if (target == null) return;
        _lockOnSystem.LockOn(target.transform);
    }

    /// <summary>
    /// ロックオンを解除する
    /// </summary>
    public void UnlockTarget() {
        _lockOnSystem.Unlock();
    }


    /// <summary>
    /// 片付け処理
    /// </summary>
    public void TearDown() {
        // プレイヤー参照をクリア
        playerTarget = null;
        _target = null;

        // 入力をリセット
        _lookInput = Vector2.zero;

        // 回転値リセット
        _currentYaw = 0f;
        _currentPitch = 0f;

        // キャッシュリセット
        _cachedCameraRotation = Quaternion.identity;
        _cachedCameraPosition = Vector3.zero;
        _cachedCameraDirection = Vector3.zero;

        // ロックオン解除
        _lockOnSystem.Unlock();
        _wasLockedOnLastFrame = false;
    }

    /// <summary>
    /// 指定オブジェクトにカメラを寄せて注視する
    /// </summary>
    public async UniTask FocusOnObject(Transform focusTarget, float focusDistance = 10f, float duration = 1.5f, float holdTime = 2f) {
        if (_isFocusing) return; // すでに演出中なら無視

        var player = CharacterUtility.GetPlayer();
        if (player != null) {
            _movement = player.GetPlayerMovement();

        }
        _movement.SetIsMoving(true);

        _isFocusing = true;
        _focusCts = new CancellationTokenSource();

        // 現在のカメラ位置と回転をキャッシュ
        _cachedCameraPosition = _camera.transform.position;
        _cachedCameraRotation = _camera.transform.rotation;

        try {
            // 寄っていくターゲット位置
            Vector3 dir = (_camera.transform.position - focusTarget.position).normalized;
            Vector3 startPos = focusTarget.position + dir * (focusDistance * 2f) + Vector3.up * 4.5f; // 少し遠目
            Vector3 endPos = focusTarget.position + dir * focusDistance + Vector3.up * 3.5f;

            Quaternion startRot = Quaternion.LookRotation(focusTarget.position - startPos, Vector3.up);
            Quaternion endRot = Quaternion.LookRotation(focusTarget.position - endPos, Vector3.up);

            // 補間で寄っていく
            float elapsed = 0f;
            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                _camera.transform.position = Vector3.Lerp(startPos, endPos, t);
                _camera.transform.rotation = Quaternion.Slerp(startRot, endRot, t);

                await UniTask.Yield(PlayerLoopTiming.Update, _focusCts.Token);
            }

            // 注視維持
            await UniTask.Delay(TimeSpan.FromSeconds(holdTime), cancellationToken: _focusCts.Token);

            // 元の位置へ戻る
            elapsed = 0f;
            while (elapsed < duration) {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                _camera.transform.position = Vector3.Lerp(endPos, _cachedCameraPosition, t);
                _camera.transform.rotation = Quaternion.Slerp(endRot, _cachedCameraRotation, t);

                _movement.SetIsMoving(false);

                await UniTask.Yield(PlayerLoopTiming.Update, _focusCts.Token);
            }
        }
        catch (OperationCanceledException) {
            // 中断された場合はそのまま
        }
        finally {
            _isFocusing = false;
            _focusCts.Dispose();
            _focusCts = null;
        }
    }



    /// <summary>
    /// オブジェクト破棄時の処理
    /// </summary>
    private void OnDestroy() {
        _inputActions?.Disable();
    }
}
