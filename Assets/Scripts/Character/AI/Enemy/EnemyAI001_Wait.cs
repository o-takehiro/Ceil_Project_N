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
    private float _randomWaitTime = -1;

    // 定数
    private const float _MIN_TIME = 3.0f;
    private const float _MAX_TIME = 5.0f;

    public override void Setup() {
        base.Setup();
        _waitTimePer = 0.0f;
        _randomWaitTime = Random.Range(_MIN_TIME, _MAX_TIME);
    }

    public override void Execute() {
        base.Execute();
        _waitTimePer += Time.deltaTime;

        //プレイヤーの方向を向く
        LookAtPlayer();

        if(_waitTimePer < _randomWaitTime) return;

        GetEnemy()._myAI.ChangeState(new EnemyAI004_Action());
    }

    public override void Teardown() {
        base.Teardown();
        _waitTimePer = 0.0f;
    }
}
