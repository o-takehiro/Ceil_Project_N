/*
 * @file    PlayerCharacter.cs
 * @brief   プレイヤーキャラクター情報
 * @author  Orui
 * @date    2025/7/8
 */
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

public class PlayerCharacter : CharacterBase {
    //現在のスピード
    public float playerMoveSpeed { get; private set; } = -1.0f;
    // カメラとの距離
    private float _cameraDistance = -1.0f;

    // プレイヤーの基礎移動スピード
    private const float _PLAYER_RAW_MOVE_SPEED = 10.0f;
    // プレイヤーの基礎ジャンプスピード
    private const float _PLAYER_JUMP_SPEED = 7f;
    // プレイヤーの重力
    private const float _PLAYER_GRAVITY = 15f;
    // プレイヤーの落ちる速度
    private const float _FALL_SPEED = 10f;
    // プレイヤーの落下開始速度
    private const float _INIT_FALL_SPEED = 2f;

    // 入力ベクトル
    private Vector2 _inputMove = Vector2.zero;
    // ジャンプ可否
    private bool _jumpRequested = false;
    // 縦方向速度
    private float _verticalVelocity = 0f;
    // Y軸の回転速度補間
    private float _turnVelocity = 0f;
    // 前フレーム接地判定
    private bool _wasGrounded = false;

    // プレイヤーTransgorm
    private readonly Transform _transform;
    // キャラクターコントローラー
    private readonly CharacterController _controller;
    // カメラ
    private readonly Camera _camera;
    // PlayerMove.cs
    private readonly PlayerMove _playerMove;

    // マスターデータ依存の変数
    public int maxMP { get; private set; } = -1;
    public int MP { get; private set; } = -1;

    public override bool isPlayer() {
        return true;
    }

    /// <summary>
    /// 使用前準備
    /// </summary>
    public override void Setup() {
        base.Setup();
        // カメラのセット

        // カメラに自身をセット
        if (CameraManager.Instance != null) {
            CameraManager.Instance.SetTarget(_transform);
        }
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="controller"></param>
    /// <param name="transform"></param>
    /// <param name="camera"></param>
    /// <param name="engineAdapter"></param>
    public PlayerCharacter(
        CharacterController controller,
        Transform transform,
        Camera camera,
        PlayerMove engineAdapter) {
        _controller = controller;
        _transform = transform;
        _camera = camera;
        _playerMove = engineAdapter;
    }

    // 外部からの入力受付
    public void SetMoveInput(Vector2 input) => _inputMove = input;
    // ジャンプ受付
    public void RequestJump() => _jumpRequested = true;

    /// <summary>
    /// 非同期処理
    /// </summary>
    /// <param name="token"></param>
    /// <returns></returns>
    public async UniTask PlayerMainLoop(CancellationToken token) {
        // 無限ループ
        while (!token.IsCancellationRequested) {
            MoveUpdate(Time.deltaTime);
            await UniTask.Yield(PlayerLoopTiming.Update, token);
        }
    }


    /// <summary>
    /// 移動関連の1フレーム分の更新
    /// </summary>
    private void MoveUpdate(float deltaTime) {
        // 地面に接地しているか
        bool isGrounded = _controller.isGrounded;

        // 着地した瞬間のめり込み対策
        if (isGrounded && !_wasGrounded) {
            _verticalVelocity = -_INIT_FALL_SPEED;
        }
        // 地面に接地していなければ降下し続ける
        else if (!isGrounded) {
            // 落下速度計算
            _verticalVelocity -= _PLAYER_GRAVITY * deltaTime;
            // 落下速度の上限設定
            if (_verticalVelocity < -_FALL_SPEED) _verticalVelocity = -_FALL_SPEED;
        }

        // ジャンプ開始
        if (_jumpRequested && isGrounded) _verticalVelocity = _PLAYER_JUMP_SPEED;

        _jumpRequested = false;
        // 1フレーム前の接地判定更新
        _wasGrounded = isGrounded;

        // カメラの角度取得
        float camY = _camera.transform.eulerAngles.y;

        Vector3 vel = new Vector3(_inputMove.x * _PLAYER_RAW_MOVE_SPEED,     // XZ平面速度
                                  _verticalVelocity,                         // 縦速度
                                  _inputMove.y * _PLAYER_RAW_MOVE_SPEED);
        // カメラの方向に向きを合わせる
        vel = Quaternion.Euler(0, camY, 0) * vel;
        // 1フレーム事の移動量
        Vector3 moveDelta = vel * deltaTime;

        // 移動処理
        _playerMove.ApplyMovement(moveDelta);
        if (_inputMove != Vector2.zero) {

            float targetAngle = -Mathf.Atan2(_inputMove.y, _inputMove.x) * Mathf.Rad2Deg + 90f;

            targetAngle += camY;

            // 滑らかに追従させる
            float angleY = Mathf.SmoothDampAngle(
                _transform.eulerAngles.y, targetAngle, ref _turnVelocity, 0.1f);
            _playerMove.ApplyRotation(Quaternion.Euler(0, angleY, 0));
        }


    }



}
