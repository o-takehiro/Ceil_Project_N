/*
 * @file    EnemyMagic.cs
 * @brief   ìGÇÃñÇñ@ÉfÅ[É^
 * @author  Riku
 * @date    2025/7/9
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class EnemyMagic : MagicBase {

	private GameObject activeMagicObject = null;

	/// <summary>
	/// ñÇñ@êwâcÇÃéÊìæ
	/// </summary>
	/// <returns></returns>
	public override eSideType GetSide() {
		return eSideType.EnemySide;
	}

	/// <summary>
	/// ñhå‰ñÇñ@
	/// </summary>
	public override void DefenseMagic(MagicObject magicObject) {
		activeMagicObject = magicObject.defense;
		activeMagicObject.transform.position = GetEnemyPosition();
		activeMagicObject.transform.rotation = GetEnemyRotation();

		Debug.Log("EEEEEEEEEEDDDDDDDDDD");
		MagicManager.instance.activeEnemyMagicID = eMagicType.Defense;
	}
	/// <summary>
	/// è¨å^íeñãñÇñ@
	/// </summary>
	public override void MiniBulletMagic(MagicObject magicObject) {
		Debug.Log("EEEEEEEEEBBBBBBBBBB");
		MagicManager.instance.activeEnemyMagicID = eMagicType.MiniBullet;
	}
}
