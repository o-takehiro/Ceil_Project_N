using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class Boss2AttackAction : MonoBehaviour {
    public void EnablePunch() {

    }

    public void DisablePunch() {

    }

    public void EnableCharge() {

    }

    public void DisableCharge() {

    }

    // 当たり判定
    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Debug.Log("パンチヒット！");
            ToPlayerDamage(GetEnemy().GetRawAttack());
        }
    }
}
