using UnityEngine;

using static CharacterUtility;
public class MageAnimationEvents : MonoBehaviour {
    [SerializeField] private Collider attackCollider;
    //[SerializeField] private Renderer AttackRenderer;
    [SerializeField] private Animator animator;
    // ダメージを与える相手
    private string _TARGET_TAG = "Enemy";
    // プレイヤーの素の攻撃力
    private int _playerRawAttack;
    void Start() {
        //AttackRenderer.enabled = false;
        if (attackCollider != null) {
            attackCollider.enabled = false; // 初期状態はオフにしておく
        }
        // プレイヤーの素の攻撃力取得
        _playerRawAttack = GetPlayer().GetRawAttack();

    }

    // 攻撃用アニメーションイベント
    public void OnCollider() {
        if (attackCollider != null) {
            attackCollider.enabled = true;
            //AttackRenderer.enabled = true;
        }
    }
    public void OffCollider() {
        if (attackCollider != null) {
            attackCollider.enabled = false;
            //AttackRenderer.enabled = false;
        }
    }

    /// <summary>
    /// ダメージを与える
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other) {
        // タグで判定
        if (other.CompareTag(_TARGET_TAG)) {
            ToEnemyDamage(_playerRawAttack);
        }
    }

}
