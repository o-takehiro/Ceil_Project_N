using UnityEngine;

public class MageAnimationEvents : MonoBehaviour {
    [SerializeField] private Collider attackCollider;
    [SerializeField] private Renderer AttackRenderer;
    [SerializeField] private Animator animator;
    void Start() {
        AttackRenderer.enabled = false;
        if (attackCollider != null) {
            attackCollider.enabled = false; // 初期状態はオフにしておく
        }
    }

    // 攻撃用アニメーションイベント
    public void OnCollider() {
        if (attackCollider != null) {
            attackCollider.enabled = true;
            AttackRenderer.enabled = true;
        }
    }
    public void OffCollider() {
        if (attackCollider != null) {
            attackCollider.enabled = false;
            AttackRenderer.enabled = false;
        }
    }

}
