using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class NormalAttackAction : StateMachineBehaviour {
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        SetEnemyAttackCollider(GetEnemy().GetEnemyAttackValue(), true);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        SetEnemyAttackCollider(GetEnemy().GetEnemyAttackValue(), false);
        GetEnemy().myAI.ChangeState(new EnemyAI001_Wait());
    }
}
