using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBossDecision : IEnemyDecision {
    /// <summary>
    /// çsìÆîªífèàóù
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public eEnemyActionType Decide(DecisionFactors factors) {
        if (factors.isCoolTime)
            return eEnemyActionType.Wait;

        else if (factors.isPlayerActiveMagic[0])
            return eEnemyActionType.MagicDefense;

        else if (factors.isPlayerClose)
            return eEnemyActionType.LeaveMove;

        else if (factors.isPlayerFar)
            return eEnemyActionType.CloseMove;

        else
            return eEnemyActionType.MiniBullet;
    }
}
