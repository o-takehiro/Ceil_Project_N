using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
using static MagicUtility;

public class TutorialBossAttackAction : MonoBehaviour {
    public void EnableAttackMagic() {
        CreateMagic(eSideType.EnemySide, GetEnemyMagicType(eMagicType.MiniBullet));
    }

    public void DisableAttackMagic() {
        CancelAllEnemyMagic();
        GetEnemy().myAI.ChangeState(new EnemyAI001_Wait());
    }
}
