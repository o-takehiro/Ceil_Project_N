using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Decision : IEnemyDecision {
    /// <summary>
    /// çsìÆîªífèàóù
    /// </summary>
    /// <param name="factors"></param>
    /// <returns></returns>
    public eEnemyActionType Decide(DecisionFactors factors) {
        if (factors.isCoolTime)
            return eEnemyActionType.Wait;

        else if (!factors.isPlayerClose && !factors.isPlayerFar)
            return eEnemyActionType.BeamAttack;

        else if (factors.isPlayerClose)
            return eEnemyActionType.LeaveMove;

        else if (factors.isPlayerFar)
            return eEnemyActionType.CloseMove;

        else
            return eEnemyActionType.Wait;
    }
}
