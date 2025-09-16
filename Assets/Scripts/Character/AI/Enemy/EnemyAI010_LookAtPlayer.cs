using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyCommonModule;

public class EnemyAI010_LookAtPlayer : CharacterAIBase<EnemyCharacter> {
    public override void Execute() {
        base.Execute();
        LookAtPlayer();
    }
}
