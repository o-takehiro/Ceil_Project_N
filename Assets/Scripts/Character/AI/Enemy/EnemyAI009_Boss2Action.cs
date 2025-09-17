using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
using static MagicUtility;

public class EnemyAI009_Boss2Action : CharacterAIBase<EnemyCharacter> {
    public override void Initialize() {
        base.Initialize();
    }

    public override void Setup() {
        base.Setup();
        GetEnemy().CancelEnemyMagic(GetEnemy().GetEnemyMagicType(eMagicType.LaserBeam));
    }

    public override void Execute() {
        base.Execute();
        float distance = GetPlayerToEnemyDistance();
        if(distance < 20) {
            AddEnemyMagicType(eMagicType.LaserBeam);
            GetEnemy().GetEnemyAnimator().SetTrigger("isMagicAttack");
            GetEnemy().myAI.ChangeState(new EnemyAI010_LookAtPlayer());
            
        } else {
            GetEnemy().myAI.ChangeState(new EnemyAI002_CloseMove());
        }
        
    }

    public override void Teardown() {
        base.Teardown();
    }
}
