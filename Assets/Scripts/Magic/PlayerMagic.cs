/*
 * @file    PlayerMagic.cs
 * @brief   プレイヤーの魔法データ
 * @author  Riku
 * @date    2025/7/9
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.Rendering;
using static CharacterUtility;

public class PlayerMagic : MagicBase {

	private float speed = 20;
	private float distanceMAX = 20;
	private float coolTime = 0.0f;
	private float coolTimeMAX = 0.5f;

	private const float SATELLITE_DISTANCE = 5;

	// 弾
	private List<GameObject> bulletList = new List<GameObject>();

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
		if (magicObject == null) return;
		Transform defense = magicObject.defense;
		defense.position = GetPlayerPosition();
		defense.rotation = GetPlayerRotation();
	}
	/// <summary>
	/// 小型弾幕魔法
	/// </summary>
	public override void MiniBulletMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		if (coolTime < 0) {
			// 未使用化不可能
			magicObject.canUnuse = false;
			Transform bullet = magicObject.GenerateMiniBullet().transform;
			bulletList.Add(bullet.gameObject);
			bullet.transform.position = GetPlayerPosition();
			bullet.transform.rotation = GetPlayerRotation();
			// 移動
			UniTask task = MiniBulletMove(magicObject, bullet);
			coolTime = coolTimeMAX;
		}
		else {
			coolTime -= Time.deltaTime;
		}
	}
	/// <summary>
	/// 小型弾幕の移動
	/// </summary>
	/// <param name="magicObject"></param>
	/// <param name="miniBullet"></param>
	/// <returns></returns>
	private async UniTask MiniBulletMove(MagicObject magicObject, Transform miniBullet) {
		float distance = 0;
		// プレイヤーから一定距離離れるまで前に進める
		while (distance < distanceMAX) {
			distance = Vector3.Distance(miniBullet.position, GetPlayerPosition());
			miniBullet.position += miniBullet.forward * speed * Time.deltaTime;
			await UniTask.DelayFrame(1);
		}
		magicObject.RemoveMiniBullet(miniBullet.gameObject);
		await UniTask.DelayFrame(1);
		// 未使用化可能
		for (int i = 0, max = bulletList.Count; i < max; i++) {
			if (bulletList[i].activeInHierarchy) return;
		}
		magicObject.canUnuse = true;
	}

	public override void SatelliteOrbital(MagicObject magicObject) {
		if (magicObject == null) return;
		// 未使用化不可能
		magicObject.canUnuse = false;
		Transform bullet = magicObject.GenerateMiniBullet().transform;
		bulletList.Add(bullet.gameObject);
		bullet.position = GetPlayerPosition();
		bullet.rotation = GetPlayerRotation();


	}
}
