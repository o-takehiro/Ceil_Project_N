using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2AttackAction : MonoBehaviour {
    public void EnableMagicBeam() {
        MagicUtility.CreateMagic(eSideType.EnemySide, eMagicType.LaserBeam);
    }

    public void DisableMagicBeam() {
        MagicUtility.MagicReset(eSideType.EnemySide, eMagicType.LaserBeam);
    }

    public void EnableDefense() {
        MagicUtility.CreateMagic(eSideType.EnemySide, eMagicType.Defense);
    }

    public void DisableDefense() {
        MagicUtility.MagicReset(eSideType.EnemySide, eMagicType.Defense);
    }
}
