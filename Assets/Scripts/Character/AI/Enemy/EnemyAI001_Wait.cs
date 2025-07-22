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
using static EnemyCommonModule;

public class EnemyAI001_Wait : CharacterAIBase<EnemyCharacter> {
    private float _waitTimePer = -1;
    public override void Setup() {
        base.Setup();
        _waitTimePer = 0.0f;
    }

    public override void Execute() {
        base.Execute();
        _waitTimePer += Time.deltaTime;

        //プレイヤーの方向を向く
        LookAtPlayer();

        if(_waitTimePer < 5) return;

        GetEnemy()._myAI.ChangeState(new EnemyAI004_Action());
    }

    public override void Teardown() {
        base.Teardown();
        _waitTimePer = 0.0f;
    }
}
