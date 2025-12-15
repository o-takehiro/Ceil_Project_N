using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBossDecision : IEnemyDecision {
    private const int DEFENSE_DISTANCE = 15;
    private const int CLOSE_DISTANCE = 25;

    public EnemyActionType Decide(EnemyContext context) {
        if (context.DistanceToPlayer < DEFENSE_DISTANCE)
            return EnemyActionType.MagicDefense;

        if (context.DistanceToPlayer > CLOSE_DISTANCE)
            return EnemyActionType.CloseMove;

        return EnemyActionType.MagicAttack;
    }
}
