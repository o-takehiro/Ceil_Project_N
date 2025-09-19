using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static CharacterUtility;

public class EnemyCommonModule {
    public static void LookAtPlayer(float setTime = 0.1f) {
        if(GetPlayer() == null || GetEnemy() == null) return;
        //プレイヤーの方向を向き続ける
        Quaternion enemyRotation = GetEnemyRotation();
        //方向を決める
        Vector3 direction = GetPlayerPosition() - GetEnemyPosition();
        //水平方向のみの回転のため、yには0を代入
        direction.y = 0;
        if(direction == Vector3.zero) return; 

        //回転させる
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        enemyRotation = Quaternion.Slerp(enemyRotation, lookRotation, setTime);
        //自身の回転に代入する
        SetEnemyRotation(enemyRotation);
    }

    public static void LookAtDirection(Vector3 setDirection) {
        //方向を決める
        Vector3 direction = setDirection;
        if (direction == Vector3.zero) return;

        //特定の方向を向き続ける
        Quaternion enemyRotation = GetEnemy().transform.rotation;
        //水平方向のみの回転のため、yには0を代入
        direction.y = 0;
        //回転させる
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        enemyRotation = Quaternion.Slerp(enemyRotation, lookRotation, 0.1f);
        //自身の回転に代入する
        SetEnemyRotation(enemyRotation);
    }
    /// <summary>
    /// カメラから見た敵が横向きになるようにする
    /// </summary>
    public static void EnemySideRotation() {
        // 今の回転を取得
        Quaternion currentRot = GetEnemy().transform.rotation;

        // Y軸周りに右へ90°回す（ローカル基準）
        Quaternion offset = Quaternion.Euler(0f, 90f, 0f);

        // ローカル回転に対して適用
        Quaternion newRot = currentRot * offset;

        // 適用
        SetEnemyRotation(newRot);
    }
}
