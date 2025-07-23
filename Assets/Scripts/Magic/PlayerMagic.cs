/*
 * @file    PlayerMagic.cs
 * @brief   プレイヤーの魔法データ
 * @author  Riku
 * @date    2025/7/9
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class PlayerMagic : MagicBase {

	private float speed = 20;
	private float distanceMAX = 20;
	private float coolTime = 1.5f;
	private float coolTimeMAX = 0.5f;

	/// <summary>
	/// 魔法陣営の取得
	/// </summary>
	/// <returns></returns>
	public override eSideType GetSide() {
		return eSideType.PlayerSide;
	}

	/// <summary>
	/// 解析魔法
	/// </summary>
	public void AnalysisMagic() {

	}
	/// <summary>
	/// 防御魔法
	/// </summary>
	public override void DefenseMagic(MagicObject magicObject) {
		Transform defense = magicObject.defense;
		defense.position = GetPlayerPosition();
		defense.rotation = GetPlayerRotation();

	}
	/// <summary>
	/// 小型弾幕魔法
	/// </summary>
	public override void MiniBulletMagic(MagicObject magicObject) {
		if (coolTime < 0){
			// 弾を生成
			GameObject bullet =  magicObject.GenerateMiniBullet();
			bullet.transform.position = GetPlayerPosition();
			bullet.transform.rotation = GetPlayerRotation();
			coolTime = coolTimeMAX;
		}
		else {
			coolTime -= Time.deltaTime;
		}
		for (int i = 0, max = magicObject.miniBulletObjects.Count; i < max; i++) {
			if (magicObject.miniBulletObjects[i] == null) continue;
			// 前に進める
			Transform magicTransform = magicObject.miniBulletObjects[i].transform;
			magicTransform.position += magicTransform.forward * speed * Time.deltaTime;
			// プレイヤーから一定の距離離れると消える
			float distance = Vector3.Distance(magicTransform.position, GetPlayerPosition());
			if (distance > distanceMAX) {
				magicObject.RemoveMiniBullet(magicObject.miniBulletObjects[i]);
			}
			else {
				magicObject.miniBulletObjects[i].transform.position = magicTransform.position;
			}
		}

	}
}
