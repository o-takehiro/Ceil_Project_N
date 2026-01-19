using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3AttackAction : MonoBehaviour {
    // í èÌçUåÇìñÇΩÇËîªíË
    [SerializeField]
    private GameObject _normalAttackCollider = null;
    // òrêUÇËâ∫ÇÎÇµçUåÇìñÇΩÇËîªíË
    [SerializeField]
    private GameObject _armDownAttackCollider = null;

    public void EnableNormalAttack() {
        if (_normalAttackCollider == null) return;

        _normalAttackCollider.SetActive(true);
    }

    public void DisableNormalAttack() {
        if (_normalAttackCollider == null) return;

        _normalAttackCollider.SetActive(false);
    }

    public void EnableArmDownAttack() {
        if (_armDownAttackCollider == null) return;

        _armDownAttackCollider.SetActive(true);
        MagicUtility.CreateMagic(eSideType.EnemySide, eMagicType.GroundShock);
    }

    public void DisableArmDownAttack() {
        if (_armDownAttackCollider == null) return;

        _armDownAttackCollider.SetActive(false);
        MagicUtility.MagicReset(eSideType.EnemySide, eMagicType.GroundShock);
    }

    public void EnableRoarAttack() {
        MagicUtility.CreateMagic(eSideType.EnemySide, eMagicType.DelayBullet);
    }

    public void DisableRoarAttack() {
        MagicUtility.MagicReset(eSideType.EnemySide, eMagicType.DelayBullet);
    }
}
