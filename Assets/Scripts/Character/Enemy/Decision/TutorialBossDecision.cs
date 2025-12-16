using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBossDecision : IEnemyDecision {
    /// <summary>
    /// 行動判断処理
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public eEnemyActionType Decide(EnemyFactors context) {
        // 距離による行動変化フラグを設定
        bool isPlayerClose = context.distanceToPlayer < context.closePlayerDistance;
        bool isPlayerFar = context.distanceToPlayer > context.farPlayerDistance;

        if (isPlayerClose && !isPlayerFar)
             return eEnemyActionType.MagicDefense;

        else if (isPlayerFar && !isPlayerClose)
            return eEnemyActionType.CloseMove;

        else if (!isPlayerClose && !isPlayerFar)
            return eEnemyActionType.MagicAttack;

        return eEnemyActionType.Wait;
    }
}
