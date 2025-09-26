using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
using static MagicUtility;

public class Boss1AttackAction : MonoBehaviour {
    [SerializeField]
    private GameObject _normalAttackCollider = null;
    [SerializeField]
    private GameObject _chargeAttackCollider = null;

    public void EnablePunch() {
        if (_normalAttackCollider == null) return;

        _normalAttackCollider.SetActive(true);
    }

    public void DisablePunch() {
        if (_normalAttackCollider == null) return;

        _normalAttackCollider.SetActive(false);
    }

    public void EnableCharge() {
        if (_chargeAttackCollider == null) return;

        _chargeAttackCollider.SetActive(true);
    }

    public void DisableCharge() {
        if (_chargeAttackCollider == null) return;

        _chargeAttackCollider.SetActive(false);
    }

    public void EnableStompAttack() {
        CreateMagic(eSideType.EnemySide, GetEnemyMagicType(eMagicType.GroundShock));
    }

    public void DisableStompAttack() {
        CancelEnemyMagic(GetEnemyMagicType(eMagicType.GroundShock));
        GetEnemy().myAI.ChangeState(new EnemyAI001_Wait());
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
