using UnityEngine;

using static CharacterUtility;

public class EnemyCommonModule {
    /// <summary>
    /// プレイヤー方向へ向く
    /// </summary>
    /// <param name="setTime"></param>
    public static void LookAtPlayer(float setTime = 0.1f, EnemyCharacter enemy = null) {
        if(!enemy || !GetPlayer()) return;

        Vector3 enemyPos = enemy.transform.position;
        Vector3 playerPos = GetPlayerPosition();

        // 目標方向
        Vector3 toPlayer = playerPos - enemyPos;
        toPlayer.y = 0f;

        if (toPlayer == Vector3.zero) return;

        // 正規化
        toPlayer.Normalize();

        // 現在の正面ベクトルを求める
        Vector3 currentForward = enemy.transform.forward;
        currentForward.y = 0f;
        currentForward.Normalize();

        // 差分角度を求める
        float angle = Vector3.SignedAngle(currentForward, toPlayer, Vector3.up);
        float rotateAngle = angle * setTime;

        // Y軸回転
        Quaternion currentRot = enemy.transform.rotation;
        Quaternion deltaRot = Quaternion.Euler(0f, rotateAngle, 0f);

        SetEnemyRotation(currentRot * deltaRot);
    }
    /// <summary>
    /// 指定された方向ベクトルへ向く
    /// </summary>
    /// <param name="setDirection"></param>
    public static void LookAtDirection(Vector3 setDirection, EnemyCharacter enemy = null) {
        if(!enemy) return;
        Vector3 direction = setDirection;
        direction.y = 0f;

        if (direction == Vector3.zero) return;

        direction.Normalize();

        // 正面ベクトルを求める
        Vector3 forward = enemy.transform.forward;
        forward.y = 0f;
        forward.Normalize();
        // 差分角度を求める
        float angle = Vector3.SignedAngle(forward, direction, Vector3.up);
        float rotateAngle = angle * 0.1f;

        // Y軸回転
        Quaternion currentRot = enemy.transform.rotation;
        Quaternion deltaRot = Quaternion.Euler(0f, rotateAngle, 0f);

        SetEnemyRotation(currentRot * deltaRot);
    }
    /// <summary>
    /// 敵自身の正面に対して横を向く
    /// </summary>
    public static void EnemySideRotation(float setTime = 0.1f, EnemyCharacter enemy = null) {
        if (!enemy) return;
        // 正面ベクトルを求める
        Vector3 currentForward = enemy.transform.forward;
        currentForward.y = 0f;
        currentForward.Normalize();
        // 右ベクトルを求める
        Vector3 targetForward = enemy.transform.right;
        targetForward.y = 0f;
        targetForward.Normalize();

        // 差分角度を求める
        float angle = Vector3.SignedAngle(currentForward,targetForward, Vector3.up);
        float rotateAngle = angle * setTime;

        // Y軸回転
        Quaternion currentRot = enemy.transform.rotation;
        Quaternion deltaRot = Quaternion.Euler(0f, rotateAngle, 0f);

        SetEnemyRotation(currentRot * deltaRot);
    }
}
