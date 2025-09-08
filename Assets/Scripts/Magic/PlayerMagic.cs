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
using Unity.Loading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;
using static CharacterUtility;

public class PlayerMagic : MagicBase {

	private float speed = 20;
	private float distanceMAX = 20;
	private float coolTime = 0.0f;
	private float coolTimeMAX = 0.5f;
	
	private bool satelliteOn = false;

	private const float SATELLITE_DISTANCE = 2;
	private const int SATELLITE_MAX = 4;

	// 小型弾幕のリスト
	private List<GameObject> bulletList = new List<GameObject>();
	// 衛星軌道のリスト
	private List<GameObject> satelliteList = new List<GameObject>();

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
		//Transform defense = magicObject.magicObjectList[(int)magicObject.activeMagic];
		Transform defense = magicObject.GenerateDefense().transform;
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
			bullet.transform.position = GetPlayerCenterPosition();
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
		while (distance < distanceMAX || miniBullet.gameObject.activeInHierarchy) {
			distance = Vector3.Distance(miniBullet.position, GetPlayerCenterPosition());
			miniBullet.rotation = GetOtherDirection(miniBullet.position);
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
	/// <summary>
	/// 相手の方向
	/// </summary>
	/// <param name="currentPos"></param>
	/// <returns></returns>
	private Quaternion GetOtherDirection(Vector3 currentPos) {
		Vector3 direction = (GetEnemyCenterPosition() - currentPos).normalized;
		return Quaternion.LookRotation(direction);
	}
	/// <summary>
	/// 衛星軌道魔法
	/// </summary>
	/// <param name="magicObject"></param>
	public override void SatelliteOrbitalMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		if (satelliteOn) return;
		satelliteOn = true;
		for (int i = 0; i < SATELLITE_MAX; i++) {
			// 未使用化不可能
			magicObject.canUnuse = false;
			Transform bullet = magicObject.GenerateMiniBullet().transform;
			satelliteList.Add(bullet.gameObject);
			// 衛星配置
			switch (i) {
				case 0:
					bullet.position += new Vector3(SATELLITE_DISTANCE, 0, 0);
					break;
				case 1:
					bullet.position += new Vector3(-SATELLITE_DISTANCE, 0, 0);
					break;
				case 2:
					bullet.position += new Vector3(0, 0, SATELLITE_DISTANCE);
					bullet.eulerAngles += new Vector3(0, 90, 0);
					break;
				case 3:
					bullet.position += new Vector3(0, 0, -SATELLITE_DISTANCE);
					bullet.eulerAngles += new Vector3(0, 90, 0);
					break;
			}
			UniTask task = SatelliteOrbitalMove(magicObject, bullet);
		}
	}
	/// <summary>
	/// 衛星軌道魔法の移動
	/// </summary>
	/// <param name="magicObject"></param>
	/// <param name="miniBullet"></param>
	/// <returns></returns>
	private async UniTask SatelliteOrbitalMove(MagicObject magicObject, Transform bullet) {
		bool loop = true;
		while (loop) {
			if (magicObject.magicObjectList[(int)eMagicType.SatelliteOrbital].transform.childCount > SATELLITE_MAX) {
                magicObject.RemoveMiniBullet(bullet.gameObject);
                return;
			}
			magicObject.transform.position = GetPlayerCenterPosition();
			Vector3 satelliteRotation = magicObject.transform.eulerAngles;
			satelliteRotation.y += speed * Time.deltaTime;
			magicObject.transform.eulerAngles = satelliteRotation;
			await UniTask.DelayFrame(1);
			loop = LoopChange();
		}
		satelliteOn = false;
		// 未使用化可能
		magicObject.canUnuse = true;
	}
	/// <summary>
	/// 衛星軌道魔法のループ抜け処理
	/// </summary>
	/// <returns></returns>
	private bool LoopChange() {
		for (int i = 0, max = satelliteList.Count; i < max; i++) {
			if (satelliteList[i].activeInHierarchy) return true;
		}
		return false;
	}
}
