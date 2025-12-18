using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttackCollider : MonoBehaviour {
    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.tag == "Player") {
            CharacterUtility.ToPlayerDamage(CharacterUtility.GetEnemy().GetRawAttack());
        }
    }
}
