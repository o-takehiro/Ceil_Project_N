using UnityEngine;

using static CharacterUtility;
using static UnityEditor.Experimental.GraphView.GraphView;

public class MageAnimationEvents : MonoBehaviour {
    [SerializeField] private Collider attackCollider;
    private PlayerAttack _playerAttack;

    void Start() {
        if (attackCollider != null) attackCollider.enabled = false;
        var player = CharacterUtility.GetPlayer();
        if (player != null) _playerAttack = player.GetAttackController();
    }

    // コライダー制御
    public void OnCollider() => attackCollider.enabled = true;
    public void OffCollider() => attackCollider.enabled = false;

    // 攻撃毎のサウンド
    public void AttackPlaySE_1() => SoundManager.Instance.PlaySE(0);
    public void AttackPlaySE_2() => SoundManager.Instance.PlaySE(1);
    public void AttackPlaySE_3() => SoundManager.Instance.PlaySE(2);

    // アニメーション内のイベント
    public void OnComboCheck() {
        _playerAttack?.TryNextCombo(); // 次段移行判定
    }
    public void OnAttackEnd() {
        _playerAttack?.EndAttack();
    }

    public void DisableAttackInput() => _playerAttack?.SetCanReceiveInput(false);
    public void EnableAttackInput() => _playerAttack?.SetCanReceiveInput(true);


    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy")) return;
        int damage = _playerAttack?.GetDamageValue() ?? 0;
        ToEnemyDamage(damage);
    }
}
