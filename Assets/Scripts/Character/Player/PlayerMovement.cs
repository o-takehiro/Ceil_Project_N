using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// プレイヤーの移動・ジャンプ処理を担当するクラス
/// </summary>
public class PlayerMovement {
    private readonly Rigidbody _rigidbody;   // 物理挙動
    private readonly Transform _transform;   // Transform
    private readonly Camera _camera;         // カメラ参照
    private readonly Animator _animator;     // アニメーション制御

    // 定数
    private const float MOVE_SPEED = 10f;    // 移動速度
    private const float JUMP_FORCE = 4.7f;   // ジャンプ力

    // 入力情報
    private Vector2 _inputMove;              // 移動入力
    private bool _jumpRequested;             // ジャンプ要求フラグ

    // 状態管理
    private bool _wasGrounded;               // 前フレームの接地状態
    private float _turnVelocity;             // 回転補間用の速度

    public PlayerMovement(Rigidbody rigidbody, Transform transform, Camera camera, Animator animator) {
        _rigidbody = rigidbody;
        _transform = transform;
        _camera = camera;
        _animator = animator;
    }

    // 移動入力を受け取る
    public void SetMoveInput(Vector2 input) => _inputMove = input;

    // ジャンプ入力を受け取る
    public void RequestJump() => _jumpRequested = true;

    /// <summary>
    /// 1フレーム分の移動処理
    /// </summary>
    public void Update(float deltaTime, bool isAttacking) {
        if (isAttacking) return; // 攻撃中は移動不可

        // 接地判定
        bool isGrounded = Physics.Raycast(
            _transform.position + Vector3.up * 0.1f,
            Vector3.down,
            0.3f,
            LayerMask.GetMask("Ground")
        );

        // ジャンプ処理
        if (_jumpRequested && isGrounded) {
            _rigidbody.AddForce(Vector3.up * JUMP_FORCE, ForceMode.VelocityChange);
            _animator.SetTrigger("jumpT");
        }
        _jumpRequested = false;
        _wasGrounded = isGrounded;

        // 待機モーション再生
        if (!isGrounded) {
            _animator.SetBool("Idle", true);
        }

        // カメラ基準でワールドベクトルで移動
        Vector3 inputDir = new Vector3(_inputMove.x, 0f, _inputMove.y).normalized;
        Vector3 moveDir = Quaternion.Euler(0f, _camera.transform.eulerAngles.y, 0f) * inputDir;

        // 移動速度をRigidbodyに適用
        Vector3 finalVelocity = moveDir * MOVE_SPEED;
        finalVelocity.y = _rigidbody.velocity.y;
        _rigidbody.velocity = finalVelocity;

        // 入力があれば回転を移動方向に合わせる
        if (inputDir.sqrMagnitude > 0.001f) {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float angleY = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref _turnVelocity, 0.1f);
            _transform.rotation = Quaternion.Euler(0f, angleY, 0f);
        }

        // 移動関連のアニメーションを再生
        AnimationHundoll(inputDir);
    }

    /// <summary>
    /// 入力に応じたアニメーション更新
    /// </summary>
    private void AnimationHundoll(Vector3 inputDir) {
        if (inputDir != Vector3.zero) {
            _animator.SetBool("Run", true);
            _animator.SetBool("Run_Stop", false);
        }
        else {
            if (_animator.GetBool("Run")) {
                _animator.SetBool("Run", false);
                _animator.SetBool("Run_Stop", true);
            }
        }
    }
}
