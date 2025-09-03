using Cysharp.Threading.Tasks.Triggers;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static CharacterUtility;

public class EnemyCommonModule {
    public static void LookAtPlayer(float setTime = 0.1f) {
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
        // カメラからプレイヤーへの方向ベクトルを取得（水平方向のみ）
        Vector3 dir = GetPlayerPosition() - Camera.main.transform.position;
        dir.y = 0;

        // もしほとんどゼロベクトルだったら処理を飛ばす（同位置の場合など）
        if (dir.sqrMagnitude < 0.001f)
            return;

        dir.Normalize(); // 水平面での方向ベクトルを正規化

        // dir に対して左向きになるベクトルを作る（Y軸回りで垂直）
        Vector3 leftDir = Vector3.Cross(Vector3.up, dir);

        // プレイヤーの前方向も水平面だけにして正規化
        Vector3 playerForward = GetPlayer().transform.forward;
        playerForward.y = 0;
        playerForward.Normalize();

        // カメラがプレイヤーの前にある場合、左向きが逆になっちゃうので反転する
        if (Vector3.Dot(dir, playerForward) < 0) {
            leftDir = -leftDir;
        }

        // 最終的に Y軸だけで向く回転を作成
        Quaternion sideRot = Quaternion.LookRotation(leftDir, Vector3.up);

        // 敵に回転を適用
        SetEnemyRotation(sideRot);
    }
}
