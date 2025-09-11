using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class BossNeck : MonoBehaviour {
    // Update is called once per frame
    void Update() {
        if (GetPlayer() != null) {
            //プレイヤーの方向を向き続ける
            Quaternion enemyRotation = transform.rotation;
            //方向を決める
            Vector3 direction = GetPlayerPosition() - GetEnemyPosition();
            //水平方向のみの回転のため、yには0を代入
            direction.y = 0;
            if (direction == Vector3.zero)
                return;

            //回転させる
            Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
            enemyRotation = Quaternion.Slerp(enemyRotation, lookRotation, 0.1f);
            //自身の回転に代入する
            transform.rotation = enemyRotation;
        }
    }
}
