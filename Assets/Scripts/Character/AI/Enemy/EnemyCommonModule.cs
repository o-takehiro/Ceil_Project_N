using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class EnemyCommonModule {
    public static void LookAtPlayer() {
        //プレイヤーの方向を向き続ける
        Quaternion enemyRotation = GetEnemyRotation();
        //方向を決める
        Vector3 direction = GetPlayerPosition() - GetEnemyPosition();
        //水平方向のみの回転のため、yには0を代入
        direction.y = 0;
        //回転させる
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        enemyRotation = Quaternion.Slerp(enemyRotation, lookRotation, 0.1f);
        //自身の回転に代入する
        SetEnemyRotation(enemyRotation);
    }

    public static void LookAtDirection(Vector3 setDirection) {
        //プレイヤーの方向を向き続ける
        Quaternion enemyRotation = GetEnemy().transform.rotation;
        //方向を決める
        Vector3 direction = setDirection;
        //水平方向のみの回転のため、yには0を代入
        direction.y = 0;
        //回転させる
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        enemyRotation = Quaternion.Slerp(enemyRotation, lookRotation, 0.1f);
        //自身の回転に代入する
        SetEnemyRotation(enemyRotation);
    }
}
