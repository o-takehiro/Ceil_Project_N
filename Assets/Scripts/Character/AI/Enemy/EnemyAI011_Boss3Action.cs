using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
using static EnemyCommonModule;
using static MagicUtility;

public class EnemyAI011_Boss3Action : CharacterAIBase<EnemyCharacter> {
    public override void Execute() {
        base.Execute();
        if(IsMagicActive(eSideType.PlayerSide, eMagicType.MiniBullet)) {
            AddEnemyMagicType(eMagicType.DelayBullet);
            GetEnemy().GetEnemyAnimator().SetTrigger("isRoarAttack");
        } else if (IsMagicActive(eSideType.PlayerSide, eMagicType.Defense)) {
            GetEnemy().GetEnemyAnimator().SetTrigger("isMagicAttack");
        } else {
            GetEnemy().GetEnemyAnimator().SetTrigger("isNormalAttack");
        }
    }
}
