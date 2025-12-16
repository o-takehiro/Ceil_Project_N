using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBoss_MiniBullet : IEnemyAction {
    /// <summary>
    /// €”õ‘Oˆ—
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy) {

    }
    /// <summary>
    /// Àsˆ—
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        enemy.GetEnemyAnimator().SetTrigger("isMagicAttack");
    }
    /// <summary>
    /// •Ğ•t‚¯ˆ—
    /// </summary>
    /// <param name="enemy"></param>
    public void Teardown(EnemyCharacter enemy) {

    }
}