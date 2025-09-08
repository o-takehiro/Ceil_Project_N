using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static CharacterUtility;

public class EnemyAI007_NormalAttack : CharacterAIBase<EnemyCharacter> {
    public override void Initialize() {
        base.Initialize();
    }
    public override void Setup() {
        base.Setup();
        GetEnemy().GetEnemyAnimator().SetTrigger("isNormalAttack");
        GetEnemy().SetEnemyAttackValue(0);
    }
    public override void Execute() {
        base.Execute();

    }
    public override void Teardown() {
        base.Teardown();
    }
}
