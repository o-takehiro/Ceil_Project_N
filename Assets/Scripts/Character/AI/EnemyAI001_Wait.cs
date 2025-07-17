/*
 * @file    EnemyAI001_Wait.cs
 * @brief   “G‚Ì‘Ò‹@AI
 * @author  Seki
 * @date    2025/7/14
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class EnemyAI001_Wait : CharacterAIBase<EnemyCharacter> {
    public override void Setup() {
        base.Setup();
    }

    public override void Execute() {
        base.Execute();
        //Ž©g‚Æ“G‚Æ‚Ì‹——£
        float distance = PlayerToEnemyDistance();
        Debug.Log(distance);
        //ƒvƒŒƒCƒ„[‚Ì•ûŒü‚ðŒü‚«‘±‚¯‚é
        Quaternion enemyRotation = GetEnemyRotation();
        //•ûŒü‚ðŒˆ‚ß‚é
        Vector3 direction = GetPlayerPosition() - GetEnemyPosition();
        //…•½•ûŒü‚Ì‚Ý‚Ì‰ñ“]‚Ì‚½‚ßAy‚É‚Í0‚ð‘ã“ü
        direction.y = 0;
        //‰ñ“]‚³‚¹‚é
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        enemyRotation = Quaternion.Slerp(enemyRotation, lookRotation, 0.1f);
        //Ž©g‚Ì‰ñ“]‚É‘ã“ü‚·‚é
        SetEnemyRotation(enemyRotation);

        if(distance > 10) GetEnemy()._myAI.ChangeState(new EnemyAI002_CloseMove());
    }

    public override void Teardown() {
        base.Teardown();
    }
}
