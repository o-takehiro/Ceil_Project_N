using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
using static MagicUtility;

public class Boss2AttackAction : MonoBehaviour {
    public void EnableMagicBeam() {
        CreateMagic(eSideType.EnemySide, GetEnemyMagicType(eMagicType.LaserBeam));
    }

    public void DisableMagicBeam() {
        CancelEnemyMagic(GetEnemyMagicType(eMagicType.LaserBeam));
        GetEnemy().myAI.ChangeState(new EnemyAI001_Wait());
    }

    public void EnableCharge() {

    }

    public void DisableCharge() {

    }
}
