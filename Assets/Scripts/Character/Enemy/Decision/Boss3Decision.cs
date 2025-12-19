using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Decision : IEnemyDecision {
    /// <summary>
    /// çsìÆîªífèàóù
    /// </summary>
    /// <param name="factors"></param>
    /// <returns></returns>
    public eEnemyActionType Decide(DecisionFactors factors) {
        if (factors.isCoolTime)
            return eEnemyActionType.Wait;

        else if (factors.isPlayerActiveMagic[1] || factors.isPlayerFar)
            return eEnemyActionType.RoarAttack;

        else if (factors.isPlayerClose && factors.isPlayerActiveMagic[0])
            return eEnemyActionType.ArmDownAttack;

        if(factors.isPlayerClose) 
            return eEnemyActionType.NormalAttack;

        else
            return eEnemyActionType.Wait;
    }
}
