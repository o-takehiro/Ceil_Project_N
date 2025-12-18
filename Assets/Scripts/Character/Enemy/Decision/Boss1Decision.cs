using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Boss1Decision : IEnemyDecision {
    /// <summary>
    /// çsìÆîªífèàóù
    /// </summary>
    /// <param name="context"></param>
    public eEnemyActionType Decide(DecisionFactors factors) {
        if (factors.isCoolTime)
            return eEnemyActionType.Wait;

        else if (factors.isPlayerClose)
            return eEnemyActionType.NormalAttack;

        else if (factors.isPlayerFar)
            return eEnemyActionType.ChargeAttack;

        else
            return eEnemyActionType.GroundShock;
    }
}
