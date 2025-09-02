using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
using static MagicUtility;

public class EnemyAI005_Boss1Action : CharacterAIBase<EnemyCharacter> {

    public override void Setup() {
        base.Setup();

    }
    public override void Execute() {
        base.Execute();
        float distance = GetPlayerToEnemyDistance();
        if(distance > 15) {
            GetEnemy()._myAI.ChangeState(new EnemyAI002_CloseMove());
        }else if (distance < 10) {
            GetEnemy()._myAI.ChangeState(new EnemyAI001_Wait());
        } else {
            GetEnemy()._myAI.ChangeState(new EnemyAI001_Wait());
        }
    }
    public override void Teardown() {
        base.Teardown();
    }
}
