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

	private bool satelliteOn = false;
    private bool beamOn = false;

    private const float SATELLITE_DISTANCE = 4;
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
		return eSideType.EnemySide;
	}

	/// <summary>
	/// 発動魔法セット
	/// </summary>
	/// <returns></returns>
	public override void SetMagicObject(MagicObject setObject) {
		useMagicObject = setObject;
	}

	/// <summary>
	/// 防御魔法
	/// </summary>
	public override void DefenseMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		Transform defense = magicObject.GenerateDefense().transform;
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
			Transform bullet = magicObject.GenerateMiniBullet().transform;
			bulletList.Add(bullet.gameObject);
			bullet.transform.position = GetEnemyCenterPosition();
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
	private async UniTask MiniBulletMove(MagicObject magicObject, Transform miniBullet) {
		float distance = 0;
		// プレイヤーから一定距離離れるまで前に進める
		while (distance < distanceMAX) {
			distance = Vector3.Distance(miniBullet.position, GetEnemyCenterPosition());
			miniBullet.position += miniBullet.forward * speed * Time.deltaTime;
			await UniTask.DelayFrame(1);
		}
		UniTask task = EffectManager.Instance.PlayEffect(eEffectType.Elimination, miniBullet.position);
		magicObject.RemoveMiniBullet(miniBullet.gameObject);
		await UniTask.DelayFrame(1);
		// 未使用化可能
		for (int i = 0, max = bulletList.Count; i < max; i++) {
			if (bulletList[i].activeInHierarchy) return;
		}
		magicObject.canUnuse = true;
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
					bullet.eulerAngles += new Vector3(90, 0, 0);
					break;
				case 3:
					bullet.position += new Vector3(0, 0, -SATELLITE_DISTANCE);
					bullet.eulerAngles += new Vector3(90, 0, 0);
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
			magicObject.transform.position = GetEnemyCenterPosition();
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
    /// <summary>
    /// ビーム(横)魔法
    /// </summary>
    /// <param name="magicObject"></param>
    public override void LaserBeamMagic(MagicObject magicObject)
    {
        if (magicObject == null) return;
        if (beamOn) return;
        beamOn = true;
        // 未使用化不可能
        magicObject.canUnuse = false;
        Transform beam = magicObject.GenerateBeam().transform;
        beam.position = GetEnemyCenterPosition();
        beam.rotation = GetEnemyRotation();
        beam.localScale = Vector3.one;
        if (GetPlayer() != null)
            beam.rotation = GetOtherDirection(beam.position);
        UniTask task = LaserBeamMove(magicObject, beam);
        task = EffectManager.Instance.PlayEffect(eEffectType.BeamShot, beam.position);
    }
    /// <summary>
    /// ビーム(横)魔法の動き
    /// </summary>
    /// <param name="magicObject"></param>
    /// <param name="beam"></param>
    /// <returns></returns>
    private async UniTask LaserBeamMove(MagicObject magicObject, Transform beam)
    {
        Vector3 beamScale = beam.localScale;
        beamScale.x = 1f;
        while (beamScale.x < 3)
        {
            beamScale = beam.localScale;
            beamScale.x += 20 * Time.deltaTime;
            beam.localScale = beamScale;
            await UniTask.DelayFrame(1);
        }
        while (beamScale.x > -1)
        {
            beamScale = beam.localScale;
            beamScale.x -= 10 * Time.deltaTime;
            beam.localScale = beamScale;
            await UniTask.DelayFrame(1);
        }
        beamOn = false;
        // 未使用化可能
        magicObject.canUnuse = true;
    }
    /// <summary>
    /// 相手の方向
    /// </summary>
    /// <param name="currentPos"></param>
    /// <returns></returns>
    private Quaternion GetOtherDirection(Vector3 currentPos)
    {
        Vector3 direction = (GetPlayerCenterPosition() - currentPos).normalized;
        return Quaternion.LookRotation(direction);
    }
}
