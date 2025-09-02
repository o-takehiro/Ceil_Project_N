using UnityEngine;

public class MageAnimationEvents : MonoBehaviour {
    [SerializeField] private Collider attackCollider;
    [SerializeField] private Renderer AttackRenderer;
    void Start() {
        AttackRenderer.enabled = false;
        if (attackCollider != null) {
            attackCollider.enabled = false; // 初期状態はオフにしておく
        }
    }

    // アニメーションイベント（攻撃開始タイミングで呼ぶ）
    public void OnCollider() {
        if (attackCollider != null) {
            attackCollider.enabled = true;
            AttackRenderer.enabled = true;
        }
    }

    // アニメーションイベント（攻撃終了タイミングで呼ぶ）
    public void OffCollider() {
        if (attackCollider != null) {
            attackCollider.enabled = false;
            AttackRenderer.enabled = false;
        }
    }
}
