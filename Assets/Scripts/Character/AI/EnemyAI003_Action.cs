using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class EnemyAI003_Action : CharacterAIBase<EnemyCharacter> {

    public override void Setup() {
        base.Setup();

    }

    public override void Execute() {
        base.Execute();
        float distance = PlayerToEnemyDistance();

        //UŒ‚‚Å‚«‚éó‘Ô‚©‚Ç‚¤‚©

        //
    }

    public override void Teardown() {
        base.Teardown();

    }
}
