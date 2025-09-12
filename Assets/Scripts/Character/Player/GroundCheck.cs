using UnityEngine;

/// <summary>
/// プレイヤーの地面接地判定
/// </summary>
public class GroundCheck : MonoBehaviour {
    public bool IsGrounded { get; private set; }

    /// <summary>
    /// 地面に触れているか
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            IsGrounded = true;
        }
    }

    /// <summary>
    /// 離れたか
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground")) {
            IsGrounded = false;
        }
    }
}