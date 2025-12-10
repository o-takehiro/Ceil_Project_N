/*
 *  @fili   PlayerMovement
 *  @author     oorui
 */

using UnityEngine;
/// <summary>
/// プレイヤーの移動・ジャンプ処理を担当するクラス
/// </summary>
public class PlayerMovement {
    private readonly Rigidbody rigidbody;       // 物理挙動
    private readonly Transform transform;       // Transform
    private readonly Camera camera;             // カメラ参照
    private readonly Animator animator;         // アニメーション制御
    private readonly GroundCheck groundCheck;   // 地面接地判定

    // 定数
    private const float _MOVE_SPEED = 10f;      // 移動速度
    private const float _JUMP_FORCE = 4.7f;     // ジャンプ力
    private const float ROTATION_SMOOTH = 0.1f; // 回転補間

    // 入力情報
    private Vector2 inputMove;              // 移動入力
    private bool jumpRequested;             // ジャンプ要求フラグ

    // 状態管理
    private float turnVelocity;  // 回転速度補間用
    private bool isJumping = false;
    public bool isDeath = false; // 死亡中は入力無効
    public bool isMoving = true; // 移動許可状態

    public bool IsGrounded => groundCheck.IsGrounded; // GroundCheckの接地判定
    public bool IsJumping => isJumping;               // ジャンプ中かどうか
    public bool CanJump => IsGrounded && !isJumping;  // 


    public PlayerMovement(Rigidbody rigidbody, Transform transform, Camera camera, Animator animator, GroundCheck groundCheck) {
        this.rigidbody = rigidbody;
        this.transform = transform;
        this.camera = camera;
        this.animator = animator;
        this.groundCheck = groundCheck;
    }

    /// <summary>
    /// 移動入力を受け取る
    /// </summary>
    /// <param name="input"></param>
    public void SetMoveInput(Vector2 input) {
        inputMove = input;
    }
    /// <summary>
    /// ジャンプ入力を受け取る
    /// </summary>
    public void RequestJump() {
        jumpRequested = true;
    }
    /// <summary>
    /// 移動可能状態にする
    /// </summary>
    public void moveSetUp() {
        isMoving = true;
    }

    /// <summary>
    /// 1フレーム分の移動処理
    /// </summary>
    public void MoveUpdate(float deltaTime, bool isAttacking) {
        // 移動不可だった場合、処理しない
        if (isAttacking || isDeath || !isMoving) return;

        // 接地判定
        bool grounded = groundCheck.IsGrounded;

        // ジャンプ処理
        JumpAction(jumpRequested, groundCheck);

        // 移動方向をカメラ基準に変換する
        Vector3 inputDir = new Vector3(inputMove.x, 0f, inputMove.y).normalized;
        Vector3 moveDir = Quaternion.Euler(0f, camera.transform.eulerAngles.y, 0f) * inputDir;

        // 移動速度をRigidbodyに適用
        Vector3 velocity = rigidbody.velocity;
        velocity.x = moveDir.x * _MOVE_SPEED;
        velocity.z = moveDir.z * _MOVE_SPEED;
        rigidbody.velocity = velocity;

        // 入力があれば回転を移動方向に合わせる
        if (inputDir.sqrMagnitude > 0.01f) {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float angleY = Mathf.SmoothDampAngle(
                transform.eulerAngles.y,
                targetAngle,
                ref turnVelocity,
                ROTATION_SMOOTH
            );
            transform.rotation = Quaternion.Euler(0f, angleY, 0f);
        }

        // 移動関連のアニメーションを再生
        UpdateAnimation(inputDir, grounded);

    }


    /// <summary>
    /// ジャンプを行う処理
    /// </summary>
    /// <param name="isJumped"></param>
    /// <param name="groundCheck"></param>
    private void JumpAction(bool isJumped, bool groundCheck) {
        // ジャンプが可能な時
        if (jumpRequested && IsGrounded && !isJumping) {
            // 上方向の速度をジャンプ力に入れる
            rigidbody.velocity = new Vector3(
                rigidbody.velocity.x,
                _JUMP_FORCE,
                rigidbody.velocity.z
            );

            // ジャンプ状態に移行
            isJumping = true;
            jumpRequested = false;

            // ジャンプアニメーション再生
            animator.SetTrigger("jumpT");
        }

        // 地面と接地したらジャンプ判定を消す
        if (IsGrounded) {
            isJumping = false;
        }

    }

    /// <summary>
    /// 入力および接地状態によるアニメーション制御
    /// </summary>
    private void UpdateAnimation(Vector3 inputDir, bool grounded) {
        bool isRunning = inputDir != Vector3.zero;

        animator.SetBool("Run", grounded && isRunning);
        animator.SetBool("Run_Stop", grounded && !isRunning);
        animator.SetBool("Idle", grounded && !isRunning);
    }

    /// <summary>
    /// 状態リセット
    /// </summary>
    public void ResetState() {
        inputMove = Vector2.zero;
        jumpRequested = false;
        isDeath = false;
        rigidbody.velocity = Vector3.zero;
    }

    /// <summary>
    /// AnimationEventで呼ぶため用のIdleアニメーション
    /// </summary>
    public void SetIdleAnimation() {
        animator.SetBool("Idle", true);
    }

}
