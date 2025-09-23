using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
using static MagicUtility;

public class Boss3AttackAction : MonoBehaviour {
    [SerializeField]
    private GameObject _normalAttackCollider = null;
    [SerializeField]
    private GameObject _armDownAttackCollider = null;

    public void EnableNormalAttack() {
        if (_normalAttackCollider == null) return;

        _normalAttackCollider.SetActive(true);
    }

    public void DisableNormalAttack() {
        if (_normalAttackCollider == null) return;

        _normalAttackCollider.SetActive(false);
    }

    public void EnableArmDownAttack() {
        if (_armDownAttackCollider == null) return;

        _armDownAttackCollider.SetActive(true);

    }

    public void DisableArmDownAttack() {
        if (_armDownAttackCollider == null) return;

        _armDownAttackCollider.SetActive(false);
    }

    public void EnableRoarAttack() {
        CreateMagic(eSideType.EnemySide, eMagicType.DelayBullet);
    }

    public void DisableRoarAttack() {
        CancelEnemyMagic(GetEnemyMagicType(eMagicType.DelayBullet));
    }

    // 当たり判定
    private void OnTriggerEnter(Collider collision) {
        if (!_normalAttackCollider.activeSelf) return; // 無効時は何もしない

        if (collision.gameObject.CompareTag("Player")) {
            Debug.Log("パンチヒット！");
            ToPlayerDamage(GetEnemy().GetRawAttack());
        }
    }
}
