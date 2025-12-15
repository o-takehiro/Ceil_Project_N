using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static MagicUtility;
using static CharacterUtility;

public class TutorialBoss_MagicDefense : IEnemyAction {
    /// <summary>
    /// é¿çsèàóù
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        AddEnemyMagicType(eMagicType.Defense);
        CreateMagic(eSideType.EnemySide, GetEnemyMagicType(eMagicType.Defense));
    }
}
