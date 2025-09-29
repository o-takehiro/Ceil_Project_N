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
using System;

public class PlayerMagic : MagicBase {
	// 弾のスピード
	private float _bulletSpeed = 30;
	// 弾の最大飛距離
	private float _bulletDistanceMax = 20;
	// 弾のクールタイム
	private float _bulletCoolTime = 0.0f;
	// 弾のクールタイムの最大
	private float _bulletCoolTimeMax = 0.3f;
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

	// 弾を一つは必ず生成させるためのフラグ	
	private bool _bulletGenerate = false;
	// 弾リセット確認用フラグ
	private bool _bulletResetCheck = false;

	// 衛星の半径
	private const float _SATELLITE_RADIUS = 2;
	// 衛星弾の最大数
	private const int _SATELLITE_MAX = 4;
	// ビームの最大飛距離
	private const float _BEAM_RANGE_MAX = 30;
	// 防御魔法の半径
	private const float _DEFENSE_RADIUS = 3;
	// 時間差弾の最大数
	private const int _DELAY_BULLET_MAX = 6;
	// 時間差弾の半径
	private const float _DELAY_BULLET_RADIUS = 3;

	// 小型弾幕のリスト
	private List<GameObject> _bulletList = new List<GameObject>();
	// 衛星軌道のリスト
	private List<GameObject> _satelliteList = new List<GameObject>();
	// 時間差弾のリスト
	private List<GameObject> _delayBulletList = new List<GameObject>();

	// カメラ
	Transform camera = Camera.main.transform;

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
		// 生成完了
		magicObject.generateFinish = true;
        // 未使用化不可能
        magicObject.canUnuse = true;
		// 防御魔法生成
		Transform defense = magicObject.GenerateDefense().transform;
		defense.position = GetPlayerPosition();
		defense.rotation = GetPlayerRotation();
		// MP消費
		ToPlayerMPDamage(0.3f);
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
		Debug.Log("BulletStart");
		// 生成完了
		magicObject.generateFinish = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		// 小型弾生成待機用実行関数
		UniTask task = MiniBulletExecute(magicObject);
	}
	/// <summary>
	/// 待機用小型弾幕魔法実行処理
	/// </summary>
	/// <returns></returns>
	private async UniTask MiniBulletExecute(MagicObject magicObject) {
		Debug.Log("MiniBulletExecute");
		do {
			if (_bulletCoolTime < 0) {
				Debug.Log("bulletGenerate");
				_bulletGenerate = true;
				// 弾が出る位置
				Vector3 activePos;
				if (magicActiveObject == null) {
					Debug.Log("GetPlayerPositionStart");
					activePos = GetPlayerCenterPosition();
					Debug.Log("GetPlayerPositionEnd");
				}
				else {
					activePos = magicActiveObject.transform.position;
				}
				// 弾生成
				Debug.Log("GenerateBulletStart");
				Transform bullet = magicObject.GenerateMiniBullet().transform;
				Debug.Log("GenerateBulletEnd");
				_bulletList.Add(bullet.gameObject);
				bullet.transform.position = activePos;
				bullet.transform.rotation = GetPlayerRotation();
				Debug.Log("GetPlayerRotation");
				// MP消費
				ToPlayerMPDamage(1);
				Debug.Log("MPDamage");
				// SE再生
				SoundManager.Instance.PlaySE(11);
				Debug.Log("PlaySE1");
				// 移動
				UniTask task = MiniBulletMove(magicObject, bullet);
				_bulletCoolTime = _bulletCoolTimeMax;
			}
			else {
				_bulletCoolTime -= Time.deltaTime;
			}
			await UniTask.Yield(PlayerLoopTiming.Update, useMagicObject.token);
		} while (!_bulletGenerate);
	}
	/// <summary>
	/// 小型弾幕の移動
	/// </summary>
	/// <param name="magicObject"></param>
	/// <param name="miniBullet"></param>
	/// <returns></returns>
	private async UniTask MiniBulletMove(MagicObject magicObject, Transform miniBullet) {
		float distance = 0;
		// カメラ方向に弾を向ける
		Vector3 cameraRotation = camera.eulerAngles;
		cameraRotation.x = 0;
		miniBullet.eulerAngles = cameraRotation;
		// プレイヤーから一定距離離れるまで前に進める
		while (distance < _bulletDistanceMax && miniBullet.gameObject.activeInHierarchy) {
			// 弾とプレイヤーの距離
			distance = Vector3.Distance(miniBullet.position, GetPlayerCenterPosition());
			Debug.Log("distance");
			// 敵がいるなら常に敵のほうに向く
			if (GetEnemy() != null)
				miniBullet.rotation = GetOtherDirection(miniBullet.position);
			// 前に進む
			miniBullet.position += miniBullet.forward * _bulletSpeed * Time.deltaTime;
			await UniTask.Yield(PlayerLoopTiming.Update, useMagicObject.token);
		}
		// エフェクト再生
		UniTask task = EffectManager.Instance.PlayEffect(eEffectType.Elimination, miniBullet.position);
		Debug.Log("PlayEffect");
		// SE再生
		SoundManager.Instance.PlaySE(9);
		Debug.Log("PlaySE2");
		// 魔法削除
		magicObject.RemoveMagic(miniBullet.gameObject);
		Debug.Log("RemoveMagic");
		// バグ回避用一時待機
		await UniTask.DelayFrame(1);
		// リセットチェックがされていなければチェック
		if (_bulletResetCheck) return;
		_bulletResetCheck = true;
		// すべてが非表示になるまで待機
		while (!UnuseCheck(_bulletList)) {
			await UniTask.Yield(PlayerLoopTiming.Update, useMagicObject.token);
		}
		// チェック終了
		_bulletResetCheck = false;
		// リストをクリア
		_bulletList.Clear();
		// 未使用化可能
		magicObject.canUnuse = true;
		Debug.Log("Bullet canUnuse");
		_bulletGenerate = false;
		//Debug.Log("PlayerMagic canUnuse" + magicObject.canUnuse);
	}
	/// <summary>
	/// 衛星軌道魔法
	/// </summary>
	/// <param name="magicObject"></param>
	public override void SatelliteOrbitalMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		if (_satelliteOn) return;
		_satelliteOn = true;
		// 生成完了
		magicObject.generateFinish = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		// 衛星弾を4つ左右前後ろに生成
		for (int i = 0; i < _SATELLITE_MAX; i++) {
			Transform bullet = magicObject.GenerateMiniBullet().transform;
			_satelliteList.Add(bullet.gameObject);
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
					// 角度調整
					bullet.eulerAngles = new Vector3(0, 90, 0);
					break;
				case 3:
					bullet.localPosition = new Vector3(0, 0, -_SATELLITE_RADIUS);
                    // 角度調整
                    bullet.eulerAngles = new Vector3(0, 90, 0);
					break;
			}
			// 衛星移動
			UniTask task = SatelliteOrbitalMove(magicObject, bullet);
		}
		// MP消費
		ToPlayerMPDamage(5);
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
		//Debug.Log("satelliteLoopStart");
		while (loop) {
			// 衛星が5つ以上なら削除
			if (magicObject.GetActiveMagicParent().childCount > _SATELLITE_MAX) {
				magicObject.RemoveMagic(bullet.gameObject);
				//Debug.Log("satelliteGoodbay");
				return;
			}
			// 常にプレイヤーの位置に
			magicObject.transform.position = GetPlayerCenterPosition();
			// 衛星の回転
			Vector3 satelliteRotation = magicObject.transform.eulerAngles;
			satelliteRotation.y += _bulletSpeed * Time.deltaTime;
			magicObject.transform.eulerAngles = satelliteRotation;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
			// ループ抜け判定
			loop = !UnuseCheck(_satelliteList);
			// プレイヤーがいないならループ抜け
			if (GetPlayer() == null) loop = false;
			//Debug.Log("satelliteLooping");
		}
		_satelliteOn = false;
		// リストクリア
		_satelliteList.Clear();
		// 未使用化可能
		magicObject.canUnuse = true;
	}
	/// <summary>
	/// ビーム(横)魔法
	/// </summary>
	/// <param name="magicObject"></param>
	public override void LaserBeamMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		if (_laserBeamOn) return;
		_laserBeamOn = true;
		// 生成完了
		magicObject.generateFinish = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		// ビームが出る位置
		Vector3 activePos;
		if (magicActiveObject == null) {
			activePos = GetPlayerCenterPosition();
		}
		else {
			activePos = magicActiveObject.transform.position;
		}
		UniTask task;
		// ビームが相手の防御魔法内で生成されるならその前に終了
		if (GetLaserBeamInDefense()) {
			task = EffectManager.Instance.PlayEffect(eEffectType.BeamDefense, activePos);
			// SE再生
			SoundManager.Instance.PlaySE(7);
			_laserBeamOn = false;
			// 未使用化可能
			magicObject.canUnuse = true;
			return;
		}
		// ビーム生成
		Transform beam = magicObject.GenerateBeam().transform;
		beam.position = activePos;
		// カメラの方に向く
		Vector3 cameraRotation = camera.eulerAngles;
		cameraRotation.x = 0;
		beam.eulerAngles = cameraRotation;
		beam.localScale = Vector3.one;
		// MP消費
		ToPlayerMPDamage(10);
		// SE再生
		SoundManager.Instance.PlaySE(6);
		// ビームが相手の防御魔法に当たるなら長さを調節
		if (GetLaserBeamDefecseHit()) {
			LaserBeamDefenseRange(beam);
		}
		// 敵がいるなら敵の方を向く
		if (GetEnemy() != null)
			beam.rotation = GetOtherDirection(beam.position);
		// ビームの動き
		task = LaserBeamMove(magicObject, beam);
		// エフェクト再生
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
	/// ビームが防御魔法の中かどうか
	/// </summary>
	/// <returns></returns>
	private bool GetLaserBeamInDefense() {
		if (GetPlayer() == null || GetEnemy() == null) return false;
		if (GetPlayerToEnemyDistance() >= _DEFENSE_RADIUS ||
			!GetMagicActive((int)eSideType.EnemySide, (int)eMagicType.Defense)) return false;
		return true;
	}
	/// <summary>
	/// ビーム(横)の長さ調整
	/// </summary>
	/// <param name="laserBeam"></param>
	private void LaserBeamDefenseRange(Transform laserBeam) {
		if (GetEnemy() == null) return;
		// 位置
		Vector3 activePos;
		if (magicActiveObject == null) {
			activePos = GetPlayerCenterPosition();
		}
		else {
			activePos = magicActiveObject.transform.position;
		}
		// 相手から自分までの方向
		Vector3 thisDirection = (activePos - GetEnemyCenterPosition()).normalized;
		// 相手の防御魔法の正面位置
		Vector3 otherDefenseForwardPos = GetEnemyCenterPosition() + thisDirection * _DEFENSE_RADIUS;
		// ビームがディフェンスに当たったまでの長さ
		float beamRange = _BEAM_RANGE_MAX - Vector3.Distance(activePos, otherDefenseForwardPos);
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
		// ビームの太さの動き
		Vector3 beamScale = beam.localScale;
		beamScale.x = 1f;
		// 拡大
		while (beamScale.x < 3) {
			beamScale = beam.localScale;
			beamScale.x += 20 * Time.deltaTime;
			beam.localScale = beamScale;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
		}
		// 縮小
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
		// 生成完了
		magicObject.generateFinish = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		// 時間差弾を6つ配置
		for (int i = 0; i < _DELAY_BULLET_MAX; i++) {
			Transform bullet = magicObject.GenerateMiniBullet().transform;
			_delayBulletList.Add(bullet.gameObject);
			// 衛星配置
			switch (i) {
				case 0:
					bullet.localPosition = new Vector3(_DELAY_BULLET_RADIUS, 0, 0);
					break;
				case 1:
					bullet.localPosition = new Vector3(-_DELAY_BULLET_RADIUS, 0, 0);
					break;
				case 2:
					bullet.localPosition = new Vector3(_DELAY_BULLET_RADIUS / 2, _DELAY_BULLET_RADIUS / 2, 0);
					break;
				case 3:
					bullet.localPosition = new Vector3(-_DELAY_BULLET_RADIUS / 2, _DELAY_BULLET_RADIUS / 2, 0);
					break;
				case 4:
					bullet.localPosition = new Vector3(_DELAY_BULLET_RADIUS / 2, -_DELAY_BULLET_RADIUS / 2, 0);
					break;
				case 5:
					bullet.localPosition = new Vector3(-_DELAY_BULLET_RADIUS / 2, -_DELAY_BULLET_RADIUS / 2, 0);
					break;
			}
			_delayBulletCoolTime = _delayBulletCoolTimeMax;
			// 時間差弾の移動
			UniTask task = DelayBulletMove(magicObject, bullet);
		}
		// MP消費
		ToPlayerMPDamage(5);
		// SE再生
		SoundManager.Instance.PlaySE(13);
	}
	/// <summary>
	/// 時間差魔法の移動
	/// </summary>
	/// <param name="magicObject"></param>
	/// <param name="delayBullet"></param>
	/// <returns></returns>
	private async UniTask DelayBulletMove(MagicObject magicObject, Transform delayBullet) {
		// 一定時間待機
		while (_delayBulletCoolTime >= 0) {
			// プレイヤーの位置、角度に合わせ続ける
			magicObject.transform.position = GetPlayerCenterPosition();
			Vector3 cameraRotation = camera.eulerAngles;
			cameraRotation.x = 0;
			magicObject.transform.eulerAngles = cameraRotation;
			_delayBulletCoolTime -= Time.deltaTime;
			await UniTask.Yield(PlayerLoopTiming.Update, useMagicObject.token);
		}
		float distance = 0;
		// プレイヤーから一定距離離れるまで前に進める
		while (distance < _bulletDistanceMax * 2 && delayBullet.gameObject.activeInHierarchy) {
            // 弾とプレイヤーの距離
            distance = Vector3.Distance(delayBullet.position, GetPlayerCenterPosition());
            // 敵がいるなら常に敵のほうに向く
			if (GetEnemy() != null)
				delayBullet.rotation = GetOtherDirection(delayBullet.position);
            // 前に進む
			delayBullet.position += delayBullet.forward * _bulletSpeed * 3 * Time.deltaTime;
			await UniTask.Yield(PlayerLoopTiming.Update, useMagicObject.token);
		}
		// エフェクト再生
		UniTask task = EffectManager.Instance.PlayEffect(eEffectType.Elimination, delayBullet.position);
		// 弾削除
		magicObject.RemoveMagic(delayBullet.gameObject);
		await UniTask.DelayFrame(1);
		// 未使用化可能
		for (int i = 0, max = _bulletList.Count; i < max; i++) {
			if (_bulletList[i].activeInHierarchy) return;
		}
		_delayBulletOn = false;
		// リストクリア
		_delayBulletList.Clear();
		// 未使用化可能
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
		// 生成完了
		magicObject.generateFinish = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		// MP消費
		ToPlayerMPDamage(30);
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
			eEffectType.Healing, GetPlayerPosition(),
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
		// 生成完了
		magicObject.generateFinish = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		// MP消費
		ToPlayerMPDamage(20);
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
		// 生成完了
		magicObject.generateFinish = true;
		// 未使用化不可能
		magicObject.canUnuse = false;

		// 待機用処理関数
		UniTask task = GroundShockExecute(magicObject);
	}
	/// <summary>
	/// 待機用衝撃波魔法実行処理
	/// </summary>
	/// <returns></returns>
	private async UniTask GroundShockExecute(MagicObject magicObject) {
		//Debug.Log("GroundShockOn");
		// 衝撃波生成
		magicObject.GenerateGroundShock();
		magicObject.transform.position = GetPlayerPosition();
		// MP消費
		ToPlayerMPDamage(15);
		// SE再生
		SoundManager.Instance.PlaySE(14);
		// エフェクト再生
		await EffectManager.Instance.PlayEffect(eEffectType.GroundShock, GetPlayerPosition());
		_groundShockOn = false;
		//Debug.Log("GroundShockOff");
		// 未使用化可能
		magicObject.canUnuse = true;
	}
	/// <summary>
	/// 大型弾幕魔法
	/// </summary>
	/// <param name="magicObject"></param>
	public override void BigBulletMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		// 生成完了
		magicObject.generateFinish = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		if (_bigBulletCoolTime < 0) {
			// 弾が出る位置
			Vector3 activePos;
			if (magicActiveObject == null) {
				activePos = GetPlayerCenterPosition();
			}
			else {
				activePos = magicActiveObject.transform.position;
			}
			// 弾生成
			Transform bullet = magicObject.GenerateMiniBullet().transform;
			_bulletList.Add(bullet.gameObject);
			bullet.transform.position = activePos;
			bullet.transform.rotation = GetPlayerRotation();
			// でかくする
			bullet.transform.localScale *= 4;
			// MP消費
			ToPlayerMPDamage(2);
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
		// カメラ方向に弾を向ける
		Vector3 cameraRotation = camera.eulerAngles;
		cameraRotation.x = 0;
		miniBullet.eulerAngles = cameraRotation;
		// プレイヤーから一定距離離れるまで前に進める
		while (distance < _bulletDistanceMax && miniBullet.gameObject.activeInHierarchy) {
            // 弾とプレイヤーの距離
            distance = Vector3.Distance(miniBullet.position, GetPlayerCenterPosition());
            // 敵がいるなら常に敵のほうに向く
			if (GetEnemy() != null)
				miniBullet.rotation = GetOtherDirection(miniBullet.position);
            // 前に進む
			miniBullet.position += miniBullet.forward * _bigBulletSpeed * Time.deltaTime;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
		}
		// エフェクト再生
		UniTask task = EffectManager.Instance.PlayEffect(eEffectType.BigElimination, miniBullet.position);
		// 弾削除
		magicObject.RemoveMagic(miniBullet.gameObject);
		await UniTask.DelayFrame(1);
		// 未使用化可能
		for (int i = 0, max = _bulletList.Count; i < max; i++) {
			if (_bulletList[i].activeInHierarchy) return;
		}
		// 未使用化可能
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
	/// 魔法の親の移動
	/// </summary>
	/// <param name="magicObject"></param>
	private async UniTask ParentObjectMove(MagicObject magicObject, Func<bool> loopCondition) {
		while (loopCondition()) {
			magicObject.transform.position = GetPlayerPosition();
			await UniTask.Yield(PlayerLoopTiming.Update, useMagicObject.token);
		}
	}
	/// <summary>
	/// リスト内のオブジェクトすべてが非表示状態かどうか
	/// </summary>
	/// <returns></returns>
	private bool UnuseCheck(List<GameObject> objectList) {
		for (int i = 0, max = objectList.Count; i < max; i++) {
			// 一つでも表示状態の物があればfalse
			if (objectList[i].activeInHierarchy) return false;
		}
		return true;
	}
}
