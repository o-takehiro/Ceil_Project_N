using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static MagicUtility;
using static CharacterUtility;

public class TutorialBoss_MagicAttack : IEnemyAction {
    /// <summary>
    /// é¿çsèàóù
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        enemy.GetEnemyAnimator().SetTrigger("isMagicAttack");
    }
}