/*
 * @file    EnemyMagic.cs
 * @brief   “G‚Ì–‚–@ƒf[ƒ^
 * @author  Riku
 * @date    2025/7/9
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class EnemyMagic : MagicBase {

	/// <summary>
	/// –‚–@w‰c‚Ìæ“¾
	/// </summary>
	/// <returns></returns>
	public override eSideType GetSide() {
		return eSideType.EnemySide;
	}

	/// <summary>
	/// –hŒä–‚–@
	/// </summary>
	public override void DefenseMagic(MagicObject magicObject) {
		Transform defense = magicObject.defense;
		defense.position = GetEnemyPosition();
		defense.rotation = GetEnemyRotation();

		//MagicManager.instance.activeEnemyMagicID = eMagicType.Defense;
	}
	/// <summary>
	/// ¬Œ^’e–‹–‚–@
	/// </summary>
	public override void MiniBulletMagic(MagicObject magicObject) {
		//MagicManager.instance.activeEnemyMagicID = eMagicType.MiniBullet;
	}
}
