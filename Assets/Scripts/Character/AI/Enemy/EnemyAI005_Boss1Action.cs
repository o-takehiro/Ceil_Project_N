using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.DualShock;
using static CharacterUtility;

public class EnemyAI005_Boss1Action : CharacterAIBase<EnemyCharacter> {

    public override void Setup() {
        base.Setup();

    }
    public override void Execute() {
        base.Execute();
        float distance = GetPlayerToEnemyDistance();
        if (distance > 50) {
            GetEnemy()._myAI.ChangeState(new EnemyAI006_Charge());
        } else if (distance < 40 && distance > 5) {
            GetEnemy()._myAI.ChangeState(new EnemyAI006_Charge());
        } else if (distance < 5) {
            GetEnemy()._myAI.ChangeState(new EnemyAI007_NormalAttack());
        }  else {
            GetEnemy()._myAI.ChangeState(new EnemyAI001_Wait());
        }
    }
    public override void Teardown() {
        base.Teardown();
    }
}
