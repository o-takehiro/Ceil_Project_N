/*
 * @file    EnemyAI001_Wait.cs
 * @brief   敵の待機AI
 * @author  Seki
 * @date    2025/7/14
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class EnemyAI001_Wait : CharacterAIBase<EnemyCharacter> {
    private float _waitTimePer = -1;
    public override void Setup() {
        base.Setup();
        _waitTimePer = 0.0f;
    }

    public override void Execute() {
        base.Execute();
        _waitTimePer += Time.deltaTime;
        //自身と敵との距離
        float distance = PlayerToEnemyDistance();
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

        if(_waitTimePer < 5) return;

        GetEnemy()._myAI.ChangeState(new EnemyAI004_Action());
    }

    public override void Teardown() {
        base.Teardown();
        _waitTimePer = 0.0f;
    }
}
