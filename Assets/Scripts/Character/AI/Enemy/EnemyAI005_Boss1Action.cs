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
        if (distance > 50 || distance < 50 &&  distance > 15) {
            GetEnemy().myAI.ChangeState(new EnemyAI006_Charge());
            AddEnemyMagicType(eMagicType.SatelliteOrbital);
            CreateMagic(eSideType.EnemySide, GetEnemyMagicType(eMagicType.SatelliteOrbital));
        } else if (distance < 15 && distance > 5) {
            GetEnemy().myAI.ChangeState(new EnemyAI010_LookAtPlayer());
            GetEnemy().GetEnemyAnimator().SetTrigger("isStompAttack");
            AddEnemyMagicType(eMagicType.GroundShock);
        } else if (distance < 5) {
            GetEnemy().myAI.ChangeState(new EnemyAI007_NormalAttack());
        }  else {
            GetEnemy().myAI.ChangeState(new EnemyAI001_Wait());
        }
    }
    public override void Teardown() {
        base.Teardown();
    }
}
