using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class Boss3AttackAction : MonoBehaviour {
    [SerializeField]
    private GameObject _normalAttackCollider = null;
    [SerializeField]
    private GameObject _armDownAttackCollider = null;

    public void EnablePunch() {
        if (_normalAttackCollider == null) return;

        _normalAttackCollider.SetActive(true);
    }

    public void DisablePunch() {
        if (_normalAttackCollider == null) return;

        _normalAttackCollider.SetActive(false);
    }

    public void EnableCharge() {
        if (_armDownAttackCollider == null) return;

        _armDownAttackCollider.SetActive(true);
    }

    public void DisableCharge() {
        if (_armDownAttackCollider == null) return;

        _armDownAttackCollider.SetActive(false);
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
