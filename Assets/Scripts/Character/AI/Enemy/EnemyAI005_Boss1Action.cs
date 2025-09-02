using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.DualShock;
using static CharacterUtility;
using static MagicUtility;

public class EnemyAI005_Boss1Action : CharacterAIBase<EnemyCharacter> {

    public override void Setup() {
        base.Setup();

    }
    public override void Execute() {
        base.Execute();
        float distance = GetPlayerToEnemyDistance();
        if(distance > 50) {
            GetEnemy()._myAI.ChangeState(new EnemyAI002_CloseMove());
        }else if (distance < 40 && distance > 0) {
            GetEnemy()._myAI.ChangeState(new EnemyAI006_Charge());
        } else {
            GetEnemy()._myAI.ChangeState(new EnemyAI001_Wait());
        }
    }
    public override void Teardown() {
        base.Teardown();
    }
}
