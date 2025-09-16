/*
 * @file    PlayerMagic.cs
 * @brief   プレイヤーの魔法データ
 * @author  Riku
 * @date    2025/7/9
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using Unity.Loading;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

using static MagicUtility;
using static CharacterUtility;

public class PlayerMagic : MagicBase {
	// 弾のスピード
	private float _speed = 20;
	// 弾の最大飛距離
	private float _distanceMAX = 20;
	// 弾のクールタイム
	private float _coolTime = 0.0f;
	// 弾のクールタイムの最大
	private float _coolTimeMAX = 0.5f;

	// 魔法の発動中フラグ
	private bool _satelliteOn = false;
	private bool _laserBeamOn = false;
	private bool _delayBulletOn = false;

	// 衛星の半径
	private const float _SATELLITE_RADIUS = 2;
	// 衛星弾の最大数
	private const int _SATELLITE_MAX = 4;
	// ビームの最大飛距離
	private const float _BEAM_RANGE_MAX = 30;
	// 防御魔法の半径
	private const float _DEFENSE_RADIUS = 3;
	// 時間差弾の最大数
	private const int _DELAY_BULLET_MAX = 5;

	// 小型弾幕のリスト
	private List<GameObject> bulletList = new List<GameObject>();
	// 衛星軌道のリスト
	private List<GameObject> satelliteList = new List<GameObject>();
	// 時間差弾のリスト
	private List<GameObject> delayBulletList = new List<GameObject>();

	/// <summary>
	/// 魔法陣営の取得
	/// </summary>
	/// <returns></returns>
	public override eSideType GetSide() {
		return eSideType.PlayerSide;
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
		if (_coolTime < 0) {
			// 未使用化不可能
			magicObject.canUnuse = false;
			Transform bullet = magicObject.GenerateMiniBullet().transform;
			bulletList.Add(bullet.gameObject);
			bullet.transform.position = GetPlayerCenterPosition();
			bullet.transform.rotation = GetPlayerRotation();
			// 移動
			UniTask task = MiniBulletMove(magicObject, bullet);
			_coolTime = _coolTimeMAX;
		}
		else {
			_coolTime -= Time.deltaTime;
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
		while (distance < _distanceMAX && miniBullet.gameObject.activeInHierarchy) {
			distance = Vector3.Distance(miniBullet.position, GetPlayerCenterPosition());
			// miniBullet.rotation = カメラローテーション
			if (GetEnemy() != null)
				miniBullet.rotation = GetOtherDirection(miniBullet.position);
			miniBullet.position += miniBullet.forward * _speed * Time.deltaTime;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
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
		if (_satelliteOn) return;
		_satelliteOn = true;
		for (int i = 0; i < _SATELLITE_MAX; i++) {
			// 未使用化不可能
			magicObject.canUnuse = false;
			Transform bullet = magicObject.GenerateMiniBullet().transform;
			satelliteList.Add(bullet.gameObject);
			// 衛星配置
			switch (i) {
				case 0:
					bullet.position += new Vector3(_SATELLITE_RADIUS, 0, 0);
					break;
				case 1:
					bullet.position += new Vector3(-_SATELLITE_RADIUS, 0, 0);
					break;
				case 2:
					bullet.position += new Vector3(0, 0, _SATELLITE_RADIUS);
					bullet.eulerAngles += new Vector3(0, 90, 0);
					break;
				case 3:
					bullet.position += new Vector3(0, 0, -_SATELLITE_RADIUS);
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
			if (magicObject.magicObjectList[(int)eMagicType.SatelliteOrbital].transform.childCount > _SATELLITE_MAX) {
				magicObject.RemoveMiniBullet(bullet.gameObject);
				return;
			}
			magicObject.transform.position = GetPlayerCenterPosition();
			Vector3 satelliteRotation = magicObject.transform.eulerAngles;
			satelliteRotation.y += _speed * Time.deltaTime;
			magicObject.transform.eulerAngles = satelliteRotation;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
			loop = LoopChange();
		}
		_satelliteOn = false;
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
	public override void LaserBeamMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		if (_laserBeamOn) return;
		_laserBeamOn = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		Transform beam = magicObject.GenerateBeam().transform;
		beam.position = GetPlayerCenterPosition();
		beam.rotation = GetPlayerRotation();
		beam.localScale = Vector3.one;
		// ビームが相手の防御魔法に当たるなら長さを調節
		if (GetLaserBeamDefecseHit()) {
			LaserBeamDefenseRange(beam);
		}
		if (GetEnemy() != null)
			beam.rotation = GetOtherDirection(beam.position);
		UniTask task = LaserBeamMove(magicObject, beam);
		task = EffectManager.Instance.PlayEffect(eEffectType.BeamShot, beam.position);
	}
	/// <summary>
	/// ビームが防御魔法に当たるかどうか
	/// </summary>
	/// <returns></returns>
	private bool GetLaserBeamDefecseHit() {
		if (GetEnemy() == null) return false;
		if (GetPlayerToEnemyDistance() - _DEFENSE_RADIUS >= _BEAM_RANGE_MAX ||
			!GetMagicActive((int)eSideType.EnemySide, (int)eMagicType.Defense)) return false;
		return true;
	}
	/// <summary>
	/// ビーム(横)の長さ調整
	/// </summary>
	/// <param name="laserBeam"></param>
	private void LaserBeamDefenseRange(Transform laserBeam) {
		if (GetEnemy() == null) return;
		// 相手から自分までの方向
		Vector3 thisDirection = (GetPlayerCenterPosition() - GetEnemyCenterPosition()).normalized;
		// 相手の防御魔法の正面位置
		Vector3 otherDefenseForwardPos = GetEnemyCenterPosition() + thisDirection * _DEFENSE_RADIUS;
		// ビームがディフェンスに当たったまでの長さ
		float beamRange = _BEAM_RANGE_MAX - Vector3.Distance(GetPlayerCenterPosition(), otherDefenseForwardPos);
		// ビームの長さを調整
		Vector3 beamScale = laserBeam.localScale;
		beamScale.z = 1 - (beamRange / _BEAM_RANGE_MAX);
		laserBeam.localScale = beamScale;
		// 当たっている位置にエフェクト生成
		UniTask task = EffectManager.Instance.PlayEffect(eEffectType.BeamDefense, otherDefenseForwardPos);

	}
	/// <summary>
	/// ビーム(横)魔法の動き
	/// </summary>
	/// <param name="magicObject"></param>
	/// <param name="beam"></param>
	/// <returns></returns>
	private async UniTask LaserBeamMove(MagicObject magicObject, Transform beam) {
		Vector3 beamScale = beam.localScale;
		beamScale.x = 1f;
		while (beamScale.x < 3) {
			beamScale = beam.localScale;
			beamScale.x += 20 * Time.deltaTime;
			beam.localScale = beamScale;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
		}
		while (beamScale.x > -1) {
			beamScale = beam.localScale;
			beamScale.x -= 10 * Time.deltaTime;
			beam.localScale = beamScale;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
		}
		_laserBeamOn = false;
		// 未使用化可能
		magicObject.canUnuse = true;
	}

	public override void DelayBulletMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		if (_delayBulletOn) return;
		_delayBulletOn = true;
		for (int i = 0; i < _DELAY_BULLET_MAX; i++) {
			// 未使用化不可能
			magicObject.canUnuse = false;
			Transform bullet = magicObject.GenerateMiniBullet().transform;
			delayBulletList.Add(bullet.gameObject);
			// 衛星配置
			switch (i) {
				case 0:
					bullet.position += new Vector3(_SATELLITE_RADIUS, 0, 0);
					break;
				case 1:
					bullet.position += new Vector3(-_SATELLITE_RADIUS, 0, 0);
					break;
				case 2:
					bullet.position += new Vector3(0, 0, _SATELLITE_RADIUS);
					bullet.eulerAngles += new Vector3(0, 90, 0);
					break;
				case 3:
					bullet.position += new Vector3(0, 0, -_SATELLITE_RADIUS);
					bullet.eulerAngles += new Vector3(0, 90, 0);
					break;
			}
			UniTask task = SatelliteOrbitalMove(magicObject, bullet);
		}
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
}
