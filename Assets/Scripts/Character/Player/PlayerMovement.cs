/*
 *  @fili   PlayerMovement
 *  @author     oorui
 */

using UnityEngine;
/// <summary>
/// プレイヤーの移動・ジャンプ処理を担当するクラス
/// </summary>
public class PlayerMovement {
    private readonly Rigidbody rigidbody;   // 物理挙動
    private readonly Transform transform;   // Transform
    private readonly Camera camera;         // カメラ参照
    private readonly Animator animator;     // アニメーション制御
    private readonly GroundCheck groundCheck;

    // 定数
    private const float MOVE_SPEED = 10f;    // 移動速度
    private const float JUMP_FORCE = 4.7f;   // ジャンプ力

    // 入力情報
    private Vector2 inputMove;              // 移動入力
    private bool jumpRequested;             // ジャンプ要求フラグ
    public bool canJump = true;

    // 状態管理
    private bool wasGrounded;               // 前フレームの接地状態
    private float turnVelocity;             // 回転補間用の速度
    public bool isGrounded;                 // 現在の接地状態
    public bool isDeath = false;            // 死亡判定
    public bool isMoving = false;           // 移動不可判定

    public PlayerMovement(Rigidbody rigidbody, Transform transform, Camera camera, Animator animator, GroundCheck groundCheck) {
        this.rigidbody = rigidbody;
        this.transform = transform;
        this.camera = camera;
        this.animator = animator;
        this.groundCheck = groundCheck;
    }

    // 移動入力を受け取る
    public void SetMoveInput(Vector2 input) => inputMove = input;

    // ジャンプ入力を受け取る
    public void RequestJump() => jumpRequested = true;

    public void moveSetUp() {
        isMoving = true;
    }

    /// <summary>
    /// 1フレーム分の移動処理
    /// </summary>
    public void MoveUpdate(float deltaTime, bool isAttacking) {
        if (isAttacking || isDeath || !isMoving) return; // 攻撃中は移動不可

        // コライダーで地面接地判定
        isGrounded = GroundCheck.IsGrounded;


        // ジャンプ処理
        if (jumpRequested && isGrounded && canJump) {
            rigidbody.AddForce(Vector3.up * JUMP_FORCE, ForceMode.VelocityChange);
            animator.SetTrigger("jumpT");
            jumpRequested = false;
            return;
        }
        jumpRequested = false;
        wasGrounded = isGrounded;

        // 待機モーション再生
        if (!isGrounded) {
            animator.SetBool("Idle", true);
        }

        // カメラ基準でワールドベクトルで移動
        Vector3 inputDir = new Vector3(inputMove.x, 0f, inputMove.y).normalized;
        Vector3 moveDir = Quaternion.Euler(0f, camera.transform.eulerAngles.y, 0f) * inputDir;

        // 移動速度をRigidbodyに適用
        Vector3 finalVelocity = moveDir * MOVE_SPEED;
        finalVelocity.y = rigidbody.velocity.y;
        rigidbody.velocity = finalVelocity;

        // 入力があれば回転を移動方向に合わせる
        if (inputDir.sqrMagnitude > 0.001f) {
            float targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;
            float angleY = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angleY, 0f);
        }

        // 移動関連のアニメーションを再生
        AnimationHundoll(inputDir);

    }

    /// <summary>
    /// 入力に応じたアニメーション更新
    /// </summary>
    private void AnimationHundoll(Vector3 inputDir) {
        if (inputDir != Vector3.zero) {
            animator.SetBool("Run", true);
            animator.SetBool("Run_Stop", false);
        }
        else {
            if (animator.GetBool("Run")) {
                animator.SetBool("Run", false);
                animator.SetBool("Run_Stop", true);
            }
        }
    }

    /// <summary>
    /// 状態リセット
    /// </summary>
    public void ResetState() {
        inputMove = Vector2.zero;
        jumpRequested = false;
        isGrounded = false;
        wasGrounded = false;
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
