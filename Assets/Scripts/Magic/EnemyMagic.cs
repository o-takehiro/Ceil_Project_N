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

	private float speed = 20;
	private float distanceMAX = 20;
	private float coolTime = 1.5f;
	private float coolTimeMAX = 0.5f;

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
		Transform defense = magicObject.defense;
		defense.position = GetEnemyPosition();
		defense.rotation = GetEnemyRotation();

		//MagicManager.instance.activeEnemyMagicID = eMagicType.Defense;
	}
	/// <summary>
	/// è¨å^íeñãñÇñ@
	/// </summary>
	public override void MiniBulletMagic(MagicObject magicObject) {
		if (coolTime < 0) {
			// íeÇê∂ê¨
			GameObject bullet = magicObject.GenerateMiniBullet();
			bullet.transform.position = GetEnemyPosition();
			bullet.transform.rotation = GetEnemyRotation();
			coolTime = coolTimeMAX;
		}
		else {
			coolTime -= Time.deltaTime;
		}
		for (int i = 0, max = magicObject.miniBulletObjects.Count; i < max; i++) {
			if (magicObject.miniBulletObjects[i] == null) continue;
			// ëOÇ…êiÇﬂÇÈ
			Transform magicTransform = magicObject.miniBulletObjects[i].transform;
			magicTransform.position += magicTransform.forward * speed * Time.deltaTime;
			// ìGÇ©ÇÁàÍíËÇÃãóó£ó£ÇÍÇÈÇ∆è¡Ç¶ÇÈ
			float distance = Vector3.Distance(magicTransform.position, GetEnemyPosition());
			if (distance > distanceMAX) {
				magicObject.RemoveMiniBullet(magicObject.miniBulletObjects[i]);
			}
			else {
				magicObject.miniBulletObjects[i].transform.position = magicTransform.position;
			}
		}
		//MagicManager.instance.activeEnemyMagicID = eMagicType.MiniBullet;
	}
}
