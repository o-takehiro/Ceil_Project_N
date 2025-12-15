using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyDecision {
    EnemyActionType Decide(EnemyContext context);
}
