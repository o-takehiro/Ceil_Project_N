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

using static MagicUtility;
using static CharacterUtility;
using System;

public class EnemyMagic : MagicBase {
	// 弾のスピード
	private float _bulletSpeed = 40;
	// 弾の最大飛距離
	private float _bulletDistanceMax = 20;
	// 弾のクールタイム
	private float _bulletCoolTime = 0f;
	// 弾のクールタイムの最大
	private float _bulletCoolTimeMax = 0.5f;
	// 時間差弾のクールタイム
	private float _delayBulletCoolTime = 0.0f;
	// 時間差弾のクールタイムの最大
	private float _delayBulletCoolTimeMax = 10.0f;
	// バフの継続時間(ミリ秒)
	private int _buffTime = 10000;
	// 大型弾のスピード
	private float _bigBulletSpeed = 15;
	// 大型弾のクールタイム
	private float _bigBulletCoolTime = 0.0f;
	// 大型弾のクールタイムの最大
	private float _bigBulletCoolTimeMax = 0.6f;

	// 魔法の発動中フラグ
	private bool _defenseOn = false;
	private bool _satelliteOn = false;
	private bool _laserBeamOn = false;
	private bool _delayBulletOn = false;
	private bool _healingOn = false;
	private bool _buffOn = false;
	private bool _groundShockOn = false;

	// 衛星の半径
	private const float _SATELLITE_RADIUS = 4;
	// 衛星弾の最大数
	private const int _SATELLITE_MAX = 4;
	// ビームの最大飛距離
	private const float _BEAM_RANGE_MAX = 30;
	// 防御魔法の半径
	private const float _DEFENSE_RADIUS = 3;
	// 時間差弾の最大数
	private const int _DELAY_BULLET_MAX = 6;
	// 時間差弾の半径
	private const float _DELAY_BULLET_RADIUS = 30;

	// 小型弾幕のリスト
	private List<GameObject> bulletList = new List<GameObject>();
	// 衛星軌道のリスト
	private List<GameObject> satelliteList = new List<GameObject>();
	// 時間差弾のリスト
	private List<GameObject> delayBulletList = new List<GameObject>();

	// カメラ
	Transform camera = Camera.main.transform;

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
        magicObject.canUnuse = true;
        Transform defense = magicObject.GenerateDefense().transform;
		defense.position = GetEnemyPosition();
		defense.rotation = GetEnemyRotation();
		if (_defenseOn) return;
		_defenseOn = true;
		// SE再生
		SoundManager.Instance.PlaySE(12);
	}
	/// <summary>
	/// 小型弾幕魔法
	/// </summary>
	public override void MiniBulletMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		if (_bulletCoolTime < 0) {
			// 未使用化不可能
			magicObject.canUnuse = false;
			Transform bullet = magicObject.GenerateMiniBullet().transform;
			bulletList.Add(bullet.gameObject);
			bullet.transform.position = GetEnemyCenterPosition();
			bullet.transform.rotation = GetEnemyRotation();
			// SE再生
			SoundManager.Instance.PlaySE(11);
			// 移動
			UniTask task = MiniBulletMove(magicObject, bullet);
			_bulletCoolTime = _bulletCoolTimeMax;
		}
		else {
			_bulletCoolTime -= Time.deltaTime;
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
		while (distance < _bulletDistanceMax) {
			distance = Vector3.Distance(miniBullet.position, GetEnemyCenterPosition());
			miniBullet.position += miniBullet.forward * _bulletSpeed * Time.deltaTime;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
		}
		UniTask task = EffectManager.Instance.PlayEffect(eEffectType.Elimination, miniBullet.position);
		SoundManager.Instance.PlaySE(9);
		magicObject.RemoveMagic(miniBullet.gameObject);
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
					bullet.localPosition = new Vector3(_SATELLITE_RADIUS, 0, 0);
					break;
				case 1:
					bullet.localPosition = new Vector3(-_SATELLITE_RADIUS, 0, 0);
					break;
				case 2:
					bullet.localPosition = new Vector3(0, 0, _SATELLITE_RADIUS);
					bullet.eulerAngles = new Vector3(0, 90, 0);
					break;
				case 3:
					bullet.localPosition = new Vector3(0, 0, -_SATELLITE_RADIUS);
					bullet.eulerAngles = new Vector3(0, 90, 0);
					break;
			}
			UniTask task = SatelliteOrbitalMove(magicObject, bullet);
		}
		// SE再生
		SoundManager.Instance.PlaySE(16);
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
			if (GetEnemy() == null) return;
			if (magicObject.GetActiveMagicParent().childCount > _SATELLITE_MAX) {
				magicObject.RemoveMagic(bullet.gameObject);
				return;
			}
			magicObject.transform.position = GetEnemyCenterPosition();
			Vector3 satelliteRotation = magicObject.transform.eulerAngles;
			satelliteRotation.y += _bulletSpeed * Time.deltaTime;
			magicObject.transform.eulerAngles = satelliteRotation;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
			loop = LoopChange();
			if (GetEnemy() == null) loop = false;
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
		UniTask task;
		_laserBeamOn = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		// ビームが相手の防御魔法内で生成されるならその前に終了
		if (GetLaserBeamInDefense()) {
			task = EffectManager.Instance.PlayEffect(eEffectType.BeamDefense, GetEnemyCenterPosition());
			// SE再生
			SoundManager.Instance.PlaySE(7);
			_laserBeamOn = false;
			// 未使用化可能
			magicObject.canUnuse = true;
			return;
		}
		Transform beam = magicObject.GenerateBeam().transform;
		beam.position = GetEnemyCenterPosition();
		beam.rotation = GetEnemyRotation();
		beam.localScale = Vector3.one;
		// SE再生
		SoundManager.Instance.PlaySE(6);
		// ビームが相手の防御魔法に当たるなら長さを調節
		if (GetLaserBeamDefecseHit()) {
			LaserBeamDefenseRange(beam);
		}
		if (GetPlayer() != null)
			beam.rotation = GetOtherDirection(GetEnemyCenterPosition());
		task = LaserBeamMove(magicObject, beam);
		task = EffectManager.Instance.PlayEffect(eEffectType.BeamShot, beam.position);
	}
	/// <summary>
	/// ビームが防御魔法に当たるかどうか
	/// </summary>
	/// <returns></returns>
	private bool GetLaserBeamDefecseHit() {
		if (GetPlayer() == null) return false;
		if (GetPlayerToEnemyDistance() - _DEFENSE_RADIUS >= _BEAM_RANGE_MAX ||
			!GetMagicActive((int)eSideType.PlayerSide, (int)eMagicType.Defense)) return false;
		return true;
	}
	/// <summary>
	/// ビームが防御魔法の中かどうか
	/// </summary>
	/// <returns></returns>
	private bool GetLaserBeamInDefense() {
		if (GetPlayer() == null || GetEnemy() == null) return false;
		if (GetPlayerToEnemyDistance() >= _DEFENSE_RADIUS ||
			!GetMagicActive((int)eSideType.PlayerSide, (int)eMagicType.Defense)) return false;
		return true;
	}
	/// <summary>
	/// ビーム(横)の長さ調整
	/// </summary>
	/// <param name="laserBeam"></param>
	private void LaserBeamDefenseRange(Transform laserBeam) {
		if (GetPlayer() == null) return;
		// 相手から自分までの方向
		Vector3 thisDirection = (GetEnemyCenterPosition() - GetPlayerCenterPosition()).normalized;
		// 相手の防御魔法の正面位置
		Vector3 otherDefenseForwardPos = GetPlayerCenterPosition() + thisDirection * _DEFENSE_RADIUS;
		// ビームがディフェンスに当たったまでの長さ
		float beamRange = _BEAM_RANGE_MAX - Vector3.Distance(GetEnemyCenterPosition(), otherDefenseForwardPos);
		// ビームの長さを調整
		Vector3 beamScale = laserBeam.localScale;
		beamScale.z = 1 - (beamRange / _BEAM_RANGE_MAX);
		laserBeam.localScale = beamScale;
		// 当たっている位置にエフェクト生成
		UniTask task = EffectManager.Instance.PlayEffect(eEffectType.BeamDefense, otherDefenseForwardPos);
		// SE再生
		SoundManager.Instance.PlaySE(7);
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
	/// <summary>
	/// 時間差弾魔法
	/// </summary>
	/// <param name="magicObject"></param>
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
					bullet.localPosition = new Vector3(_DELAY_BULLET_RADIUS / 2, 0, 0);
					break;
				case 1:
					bullet.localPosition = new Vector3(-_DELAY_BULLET_RADIUS / 2, 0, 0);
					break;
				case 2:
					bullet.localPosition = new Vector3(_DELAY_BULLET_RADIUS, _DELAY_BULLET_RADIUS / 2, 0);
					break;
				case 3:
					bullet.localPosition = new Vector3(-_DELAY_BULLET_RADIUS, _DELAY_BULLET_RADIUS / 2, 0);
					break;
				case 4:
					bullet.localPosition = new Vector3(_DELAY_BULLET_RADIUS / 2, _DELAY_BULLET_RADIUS, 0);
					break;
				case 5:
					bullet.localPosition = new Vector3(-_DELAY_BULLET_RADIUS / 2, _DELAY_BULLET_RADIUS, 0);
					break;
			}
			bullet.localScale *= 10;
			_delayBulletCoolTime = _delayBulletCoolTimeMax;
			// SE再生
			SoundManager.Instance.PlaySE(13);
			UniTask task = DelayBulletMove(magicObject, bullet);
		}
	}
	/// <summary>
	/// 時間差魔法の移動
	/// </summary>
	/// <param name="magicObject"></param>
	/// <param name="delayBullet"></param>
	/// <returns></returns>
	private async UniTask DelayBulletMove(MagicObject magicObject, Transform delayBullet) {
		while (_delayBulletCoolTime >= 0) {
			magicObject.transform.position = GetEnemyCenterPosition();
			magicObject.transform.rotation = GetEnemyRotation();
			_delayBulletCoolTime -= Time.deltaTime;
			await UniTask.Yield(PlayerLoopTiming.Update, useMagicObject.token);
		}
		float distance = 0;
		// 一定距離離れるまで前に進める
		while (distance < _bulletDistanceMax * 2 && delayBullet.gameObject.activeInHierarchy) {
			distance = Vector3.Distance(delayBullet.position, GetEnemyCenterPosition());
			if (GetEnemy() != null)
				delayBullet.rotation = GetOtherDirection(delayBullet.position);
			delayBullet.position += delayBullet.forward * _bulletSpeed * 3 * Time.deltaTime;
			await UniTask.Yield(PlayerLoopTiming.Update, useMagicObject.token);
		}
		UniTask task = EffectManager.Instance.PlayEffect(eEffectType.BigElimination, delayBullet.position);
		magicObject.RemoveMagic(delayBullet.gameObject);
		await UniTask.DelayFrame(1);
		// 未使用化可能
		for (int i = 0, max = bulletList.Count; i < max; i++) {
			if (bulletList[i].activeInHierarchy) return;
		}
		_delayBulletOn = false;
		magicObject.canUnuse = true;
	}
	/// <summary>
	/// 回復魔法
	/// </summary>
	/// <param name="magicObject"></param>
	public override void HealingMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		if (_healingOn) return;
		_healingOn = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		// SE再生
		SoundManager.Instance.PlaySE(15);
		// 待機用処理関数
		UniTask task = HealingExecute(magicObject);
		task = ParentObjectMove(magicObject, () => _healingOn);
	}
	/// <summary>
	/// 待機用回復魔法実行処理
	/// </summary>
	/// <returns></returns>
	private async UniTask HealingExecute(MagicObject magicObject) {
		// 親オブジェクトを指定してエフェクト再生
		await EffectManager.Instance.PlayEffect(
			eEffectType.Healing, GetEnemyPosition(),
			magicObject.GetActiveMagicParent());
		_healingOn = false;
		// 未使用化可能
		magicObject.canUnuse = true;
	}
	/// <summary>
	/// バフ魔法
	/// </summary>
	/// <param name="magicObject"></param>
	public override void BuffMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		if (_buffOn) return;
		_buffOn = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		// SE再生
		SoundManager.Instance.PlaySE(8);
		// 待機用処理関数
		UniTask task = BuffExecute(magicObject);
		task = ParentObjectMove(magicObject, () => _buffOn);
	}
	/// <summary>
	/// 待機用バフ魔法実行処理
	/// </summary>
	/// <returns></returns>
	private async UniTask BuffExecute(MagicObject magicObject) {
		magicObject.GenerateBuff();
		await UniTask.Delay(_buffTime, false, PlayerLoopTiming.Update, useMagicObject.token);
		_buffOn = false;
		// 未使用化可能
		magicObject.canUnuse = true;
	}
	/// <summary>
	/// 衝撃波魔法
	/// </summary>
	/// <param name="magicObject"></param>
	public override void GroundShockMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		if (_groundShockOn) return;
		_groundShockOn = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		magicObject.transform.position = GetEnemyPosition();
		magicObject.GenerateGroundShock();
		// SE再生
		SoundManager.Instance.PlaySE(14);
		// 待機用処理関数
		UniTask task = GroundShockExecute(magicObject);
	}
	/// <summary>
	/// 待機用衝撃波魔法実行処理
	/// </summary>
	/// <returns></returns>
	private async UniTask GroundShockExecute(MagicObject magicObject) {
		// エフェクト再生
		await EffectManager.Instance.PlayEffect(eEffectType.GroundShock, GetEnemyPosition());
		_groundShockOn = false;
		// 未使用化可能
		magicObject.canUnuse = true;
	}
	/// <summary>
	/// 大型弾幕魔法
	/// </summary>
	/// <param name="magicObject"></param>
	public override void BigBulletMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		if (_bigBulletCoolTime < 0) {
			// 未使用化不可能
			magicObject.canUnuse = false;
			Vector3 activePos;
			if (magicActiveObject == null) {
				activePos = GetPlayerCenterPosition();
			}
			else {
				activePos = magicActiveObject.transform.position;
			}
			Transform bullet = magicObject.GenerateMiniBullet().transform;
			bulletList.Add(bullet.gameObject);
			bullet.transform.position = activePos;
			bullet.transform.rotation = GetPlayerRotation();
			bullet.transform.localScale *= 4;
			// SE再生
			SoundManager.Instance.PlaySE(11);
			// 移動
			UniTask task = BigBulletMove(magicObject, bullet);
			_bigBulletCoolTime = _bigBulletCoolTimeMax;
		}
		else {
			_bigBulletCoolTime -= Time.deltaTime;
		}
	}
	/// <summary>
	/// 大型弾幕の移動
	/// </summary>
	/// <param name="magicObject"></param>
	/// <param name="miniBullet"></param>
	/// <returns></returns>
	private async UniTask BigBulletMove(MagicObject magicObject, Transform miniBullet) {
		float distance = 0;
		// プレイヤーから一定距離離れるまで前に進める
		while (distance < _bulletDistanceMax && miniBullet.gameObject.activeInHierarchy) {
			distance = Vector3.Distance(miniBullet.position, GetPlayerCenterPosition());
			miniBullet.rotation = camera.rotation;
			if (GetEnemy() != null)
				miniBullet.rotation = GetOtherDirection(miniBullet.position);
			miniBullet.position += miniBullet.forward * _bigBulletSpeed * Time.deltaTime;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
		}
		UniTask task = EffectManager.Instance.PlayEffect(eEffectType.Elimination, miniBullet.position);
		magicObject.RemoveMagic(miniBullet.gameObject);
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
		Vector3 direction = (GetPlayerCenterPosition() - currentPos).normalized;
		return Quaternion.LookRotation(direction);
	}
	/// <summary>
	/// 魔法の親の移動
	/// </summary>
	/// <param name="magicObject"></param>
	private async UniTask ParentObjectMove(MagicObject magicObject, Func<bool> loopCondition) {
		while (loopCondition()) {
			magicObject.transform.position = GetEnemyPosition();
			await UniTask.Yield(PlayerLoopTiming.Update, useMagicObject.token);
		}
	}
}
