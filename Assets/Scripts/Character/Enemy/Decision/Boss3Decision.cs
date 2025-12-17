using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Decision : IEnemyDecision {
    /// <summary>
    /// s“®”»’fˆ—
    /// </summary>
    /// <param name="factors"></param>
    /// <returns></returns>
    public eEnemyActionType Decide(DecisionFactors factors) {
        if (factors.isCoolTime)
            return eEnemyActionType.Wait;

        return eEnemyActionType.Wait;
    }
}
