/*
 * @file    EnemyMagic.cs
 * @brief   敵の魔法データ
 * @author  Riku
 * @date    2025/7/9
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class EnemyMagic : MagicBase {

	private float speed = 20;
	private float distanceMAX = 20;
	private float coolTime = 0f;
	private float coolTimeMAX = 0.5f;

	// 弾
	List<GameObject> bulletList = new List<GameObject>();

	/// <summary>
	/// 魔法陣営の取得
	/// </summary>
	/// <returns></returns>
	public override eSideType GetSide() {
		return eSideType.EnemySide;
	}

	/// <summary>
	/// 防御魔法
	/// </summary>
	public override void DefenseMagic(MagicObject magicObject) {
		Transform defense = magicObject.magicObjectList[(int)magicObject.activeMagic];
		defense.position = GetEnemyPosition();
		defense.rotation = GetEnemyRotation();

		//MagicManager.instance.activeEnemyMagicID = eMagicType.Defense;
	}
	/// <summary>
	/// 小型弾幕魔法
	/// </summary>
	public override void MiniBulletMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		if (coolTime < 0) {
			// 未使用化不可能
			magicObject.canUnuse = false;
			GameObject bullet = magicObject.GenerateMiniBullet();
			bulletList.Add(bullet);
			bullet.transform.position = GetEnemyPosition();
			bullet.transform.rotation = GetEnemyRotation();
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
	private async UniTask MiniBulletMove(MagicObject magicObject, GameObject miniBullet) {
		Transform magicTransform = miniBullet.transform;
		float distance = 0;
		// プレイヤーから一定距離離れるまで前に進める
		while (distance < distanceMAX) {
			distance = Vector3.Distance(magicTransform.position, GetEnemyPosition());
			magicTransform.position += magicTransform.forward * speed * Time.deltaTime;
			miniBullet.transform.position = magicTransform.position;
			await UniTask.DelayFrame(1);
		}
		magicObject.RemoveMiniBullet(miniBullet);
		await UniTask.DelayFrame(1);
		// 未使用化可能
		for (int i = 0, max = bulletList.Count; i < max; i++) {
			if (bulletList[i].activeInHierarchy) return;
		}
		magicObject.canUnuse = true;
	}

    public override void SatelliteOrbitalMagic(MagicObject magicObject) {
        
    }
}
