using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class EnemyAttackCollider : MonoBehaviour {
    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.tag == "Player") {
            ToPlayerDamage(GetEnemy().GetRawAttack());
        }
    }
}
