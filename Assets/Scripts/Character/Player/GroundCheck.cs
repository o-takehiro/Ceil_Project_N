/*
 *  @file   GroundCheck
 *  @author oorui
 */
using UnityEngine;

/// <summary>
/// プレイヤーの地面接地判定
/// </summary>
public class GroundCheck : MonoBehaviour {
    [SerializeField] private LayerMask groundLayer;         // 地面レイヤー
    [SerializeField] private float sphereRadius = 0.25f;    // 足元の半径
    [SerializeField] private float checkDistance = 0.1f;    // 判定距離
    [SerializeField] private float coyoteTime = 0.1f;       // 接地猶予時間

    private float lastGroundTime;   // 最後に地面に触れた時刻
    private bool isTouchingGround;  // 地面に触れたかどうか

    /// <summary>
    /// 接地判定
    /// </summary>
    public bool IsGrounded =>
        isTouchingGround ||
        (Time.time - lastGroundTime) <= coyoteTime;

    /// <summary>
    /// 更新処理
    /// </summary>
    private void FixedUpdate() {
        // 接地判定
        SphereCastGround();

    }
    /// <summary>
    /// 下方向にSphereCastして、判定をする
    /// </summary>
    private void SphereCastGround() {
        Vector3 origin = transform.position;

        // SphererCast
        if (Physics.SphereCast(origin, sphereRadius, Vector3.down,
                    out RaycastHit hit, checkDistance, groundLayer)) {
            lastGroundTime = Time.time;
        }
    }

    /// <summary>
    /// 地面に触れたとき、接地判定にする
    /// </summary>
    private void OnTriggerEnter(Collider other) {
        if (!IsGround(other)) return;
        // 接地中にする
        isTouchingGround = true;
        lastGroundTime = Time.time;
    }

    /// <summary>
    /// 地面に触れ続けている間、接地判定を継続
    /// </summary>
    private void OnTriggerStay(Collider other) {
        if (IsGround(other)) {
            // 接地中は刑ぞｋ
            isTouchingGround = true;
            lastGroundTime = Time.time;
        }
    }

    /// <summary>
    /// 地面から離れたとき、接地カウント減少
    /// </summary>
    private void OnTriggerExit(Collider other) {
        if (!IsGround(other)) return;
        // 接地判定を解除する
        isTouchingGround = false;
    }

    /// <summary>
    /// レイヤーが地面かチェック
    /// </summary>
    private bool IsGround(Collider other) {
        return ((1 << other.gameObject.layer) & groundLayer) != 0;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Vector3 origin = transform.position + Vector3.up * sphereRadius;
        Gizmos.DrawWireSphere(origin + Vector3.down * checkDistance, sphereRadius);
    }
}