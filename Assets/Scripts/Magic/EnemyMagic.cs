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
	// 弾のクールタイム
	private float _bulletCoolTime = 0f;
	// 時間差弾のクールタイム
	private float _delayBulletCoolTime = 0.0f;
	// バフの継続時間(ミリ秒)
	private int _buffTime = 10000;
	// 大型弾のスピード
	private float _bigBulletSpeed = 15;
	// 大型弾のクールタイム
	private float _bigBulletCoolTime = 0.0f;

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

	// 弾の最大飛距離
	private float _BULLET_DISTANSE_MAX = 20;
    // 弾の最大飛距離
    private float _DELAY_BULLET_DISTANSE_MAX = 40;
    // 弾のクールタイムの最大
    private float _BULLET_COOL_TIME_MAX = 0.5f;
	// 時間差弾のクールタイムの最大
	private float _DELAY_BULLET_COOL_TIME_MAX = 10.0f;
	// 大型弾のクールタイムの最大
	private float _BIG_BULLET_COOL_TIME_MAX = 0.6f;
	// 衛星の半径
	private const float _SATELLITE_RADIUS = 4;
	// 衛星弾の最大数
	private const int _SATELLITE_MAX = 4;
	// ビームの最大飛距離
	private const float _BEAM_RANGE_MAX = 30;
    // ビームの最大サイズ
    private float _BEAM_SCALE_MAX = 3;
    // ビームの最小サイズ
    private float _BEAM_SCALE_MIN = -1;
    // ビームの拡大倍率
    private float _BEAM_ENLARGEMENT_RATE = 20;
    // ビームの縮小倍率
    private float _BEAM_REDUCTION_RATE = 10;
    // 防御魔法の半径
    private const float _DEFENSE_RADIUS = 3;
	// 時間差弾の最大数
	private const int _DELAY_BULLET_MAX = 6;
	// 時間差弾の半径
	private const float _DELAY_BULLET_RADIUS = 30;
    // ボス用のサイズ倍率
    private const float _BOSS_DELAY_BULLET_SCALE_RATIO = 10;
    // 大型弾用のサイズ倍率
    private const float _BIG_BULLET_SCALE_RATIO = 4;
    // 90度回転
    private const int _DEGREE_RIGHT_ANGLE = 90;

	// 小型弾幕のリスト
	private List<GameObject> _bulletList = new List<GameObject>();
	// 衛星軌道のリスト
	private List<GameObject> _satelliteList = new List<GameObject>();
	// 時間差弾のリスト
	private List<GameObject> _delayBulletList = new List<GameObject>();

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
		// 生成完了
		magicObject.generateFinish = true;
        // 未使用化不可能
		magicObject.canUnuse = true;
        // 防御魔法生成
        Transform defense = magicObject.GenerateDefense().transform;
		defense.position = GetEnemyPosition();
		defense.rotation = GetEnemyRotation();
		if (_defenseOn) return;
		_defenseOn = true;
		// SE再生
		SoundManager.Instance.PlaySE((int)eSEType.DefenseActive);
    }
    /// <summary>
    /// 小型弾幕魔法
    /// </summary>
    public override void MiniBulletMagic(MagicObject magicObject) {
        if (magicObject == null) return;
        // 生成完了
        magicObject.generateFinish = true;
        // 小型弾生成待機用実行関数
        UniTask task = MiniBulletExecute(magicObject);
    }
    /// <summary>
    /// 待機用小型弾幕魔法実行処理
    /// </summary>
    /// <returns></returns>
    private async UniTask MiniBulletExecute(MagicObject magicObject) {
        do {
            if (_bulletCoolTime < 0) {
				_bulletGenerate = true;
				// 未使用化不可能
				magicObject.canUnuse = false;
                // 弾生成
                Transform bullet = magicObject.GenerateMiniBullet().transform;
                _bulletList.Add(bullet.gameObject);
                bullet.transform.position = GetEnemyCenterPosition();
                bullet.transform.rotation = GetEnemyRotation();
                // SE再生
                SoundManager.Instance.PlaySE((int)eSEType.BulletShot);
                // 移動
                UniTask task = MiniBulletMove(magicObject, bullet);
                _bulletCoolTime = _BULLET_COOL_TIME_MAX;
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
		// 敵から一定距離離れるまで前に進める
		while (distance < _BULLET_DISTANSE_MAX) {
			// 弾と敵の距離
			distance = Vector3.Distance(miniBullet.position, GetEnemyCenterPosition());
			// 前に進む
			miniBullet.position += miniBullet.forward * _bulletSpeed * Time.deltaTime;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
		}
		// エフェクト再生
		UniTask task = EffectManager.Instance.PlayEffect(eEffectType.Elimination, miniBullet.position);
		// SE再生
		SoundManager.Instance.PlaySE((int)eSEType.BulletElimination);
		// 魔法削除
		magicObject.RemoveMagic(miniBullet.gameObject);
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
		magicObject.canUnuse = true;

        // リセットチェックがされていなければチェック
        if (_bulletResetCheck) return;
        // 未使用化可能
        _bulletResetCheck = true;
        _bulletGenerate = false;
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
					bullet.eulerAngles = new Vector3(0, _DEGREE_RIGHT_ANGLE, 0);
					break;
				case 3:
					bullet.localPosition = new Vector3(0, 0, -_SATELLITE_RADIUS);
                    // 角度調整
					bullet.eulerAngles = new Vector3(0, _DEGREE_RIGHT_ANGLE, 0);
					break;
			}
            // 衛星移動
			UniTask task = SatelliteOrbitalMove(magicObject, bullet);
		}
		// SE再生
		SoundManager.Instance.PlaySE((int)eSEType.SatelliteActive);
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
            // 衛星が5つ以上なら削除
            if (magicObject.GetActiveMagicParent().childCount > _SATELLITE_MAX) {
				magicObject.RemoveMagic(bullet.gameObject);
				return;
			}
            // 常にプレイヤーの位置に
            magicObject.transform.position = GetEnemyCenterPosition();
            // 衛星の回転
            Vector3 satelliteRotation = magicObject.transform.eulerAngles;
			satelliteRotation.y += _bulletSpeed * Time.deltaTime;
			magicObject.transform.eulerAngles = satelliteRotation;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
            // ループ抜け判定
            loop = !UnuseCheck(_satelliteList);
            // プレイヤーがいないならループ抜け
            if (GetEnemy() == null) loop = false;
		}
		_satelliteOn = false;
		// リストをクリア
		_satelliteList.Clear();
		// 未使用化可能
		magicObject.canUnuse = true;
	}
	/// <summary>
	/// ビーム魔法
	/// </summary>
	/// <param name="magicObject"></param>
	public override void LaserBeamMagic(MagicObject magicObject) {
		if (magicObject == null) return;
		if (_laserBeamOn) return;
		UniTask task;
		_laserBeamOn = true;
		// 生成完了
		magicObject.generateFinish = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		// ビームが相手の防御魔法内で生成されるならその前に終了
		if (GetLaserBeamInDefense()) {
			task = EffectManager.Instance.PlayEffect(eEffectType.BeamDefense, GetEnemyCenterPosition());
			// SE再生
			SoundManager.Instance.PlaySE((int)eSEType.BeamDefense);
			_laserBeamOn = false;
			// 未使用化可能
			magicObject.canUnuse = true;
			return;
		}
        // ビーム生成
        Transform beam = magicObject.GenerateBeam().transform;
        // カメラの方に向く
        beam.position = GetEnemyCenterPosition();
		beam.rotation = GetEnemyRotation();
		beam.localScale = Vector3.one;
		// SE再生
		SoundManager.Instance.PlaySE((int)eSEType.Beam);
		// ビームが相手の防御魔法に当たるなら長さを調節
		if (GetLaserBeamDefenseHit()) {
			LaserBeamDefenseRange(beam);
		}
		// プレイヤーがいるならプレイヤーの方を向く
		if (GetPlayer() != null)
			beam.rotation = GetOtherDirection(GetEnemyCenterPosition());
		// ビームの動き
		task = LaserBeamMove(magicObject, beam);
		// エフェクト再生
		task = EffectManager.Instance.PlayEffect(eEffectType.BeamShot, beam.position);
	}
	/// <summary>
	/// ビームが防御魔法に当たるかどうか
	/// </summary>
	/// <returns></returns>
	private bool GetLaserBeamDefenseHit() {
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
	/// ビームの長さ調整
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
		SoundManager.Instance.PlaySE((int)eSEType.BeamDefense);
	}
	/// <summary>
	/// ビーム魔法の動き
	/// </summary>
	/// <param name="magicObject"></param>
	/// <param name="beam"></param>
	/// <returns></returns>
	private async UniTask LaserBeamMove(MagicObject magicObject, Transform beam) {
		// ビームの太さの動き
		Vector3 beamScale = beam.localScale;
		beamScale.x = 1;
		// 拡大
		while (beamScale.x < _BEAM_SCALE_MAX) {
			beamScale = beam.localScale;
			beamScale.x += _BEAM_ENLARGEMENT_RATE * Time.deltaTime;
			beam.localScale = beamScale;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
		}
		// 縮小
		while (beamScale.x > _BEAM_SCALE_MIN) {
			beamScale = beam.localScale;
			beamScale.x -= _BEAM_REDUCTION_RATE * Time.deltaTime;
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
			// ボス用に拡大
			bullet.localScale *= _BOSS_DELAY_BULLET_SCALE_RATIO;
			_delayBulletCoolTime = _DELAY_BULLET_COOL_TIME_MAX;
			// 時間差弾の移動
			UniTask task = DelayBulletMove(magicObject, bullet);
		}
		// SE再生
		SoundManager.Instance.PlaySE((int)eSEType.DelayAction);
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
			// 敵の位置、角度に合わせ続ける
			magicObject.transform.position = GetEnemyCenterPosition();
			magicObject.transform.rotation = GetEnemyRotation();
			_delayBulletCoolTime -= Time.deltaTime;
			await UniTask.Yield(PlayerLoopTiming.Update, useMagicObject.token);
		}
		float distance = 0;
		// 一定距離離れるまで前に進める
		while (distance < _DELAY_BULLET_DISTANSE_MAX && delayBullet.gameObject.activeInHierarchy) {
			// 弾と敵の距離
			distance = Vector3.Distance(delayBullet.position, GetEnemyCenterPosition());
			// プレイヤーのほうに向く
			if (GetEnemy() != null)
				delayBullet.rotation = GetOtherDirection(delayBullet.position);
			// 前に進む
			delayBullet.position += delayBullet.forward * _bulletSpeed * 3 * Time.deltaTime;
			await UniTask.Yield(PlayerLoopTiming.Update, useMagicObject.token);
		}
		// エフェクト再生
		UniTask task = EffectManager.Instance.PlayEffect(eEffectType.BigElimination, delayBullet.position);
		// 弾削除
		magicObject.RemoveMagic(delayBullet.gameObject);
		await UniTask.DelayFrame(1);
		// 未使用化可能
		for (int i = 0, max = _bulletList.Count; i < max; i++) {
			if (_bulletList[i].activeInHierarchy) return;
		}
		_delayBulletOn = false;
		// リストをクリア
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
		// SE再生
		SoundManager.Instance.PlaySE((int)eSEType.Healing);
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
		// 生成完了
		magicObject.generateFinish = true;
		// 未使用化不可能
		magicObject.canUnuse = false;
		// SE再生
		SoundManager.Instance.PlaySE((int)eSEType.Buff);
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
		// 衝撃波生成
        magicObject.GenerateGroundShock();
        magicObject.transform.position = GetEnemyPosition();
        // SE再生
        SoundManager.Instance.PlaySE((int)eSEType.GroundShock);
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
			bullet.transform.localScale *= _BIG_BULLET_SCALE_RATIO;
			// SE再生
			SoundManager.Instance.PlaySE((int)eSEType.BulletShot);
			// 移動
			UniTask task = BigBulletMove(magicObject, bullet);
			_bigBulletCoolTime = _BIG_BULLET_COOL_TIME_MAX;
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
		while (distance < _BULLET_DISTANSE_MAX && miniBullet.gameObject.activeInHierarchy) {
			// 敵とプレイヤーの距離
			distance = Vector3.Distance(miniBullet.position, GetPlayerCenterPosition());
			// 
			miniBullet.rotation = GetEnemyRotation();
			if (GetEnemy() != null)
				miniBullet.rotation = GetOtherDirection(miniBullet.position);
			// 前に進む
			miniBullet.position += miniBullet.forward * _bigBulletSpeed * Time.deltaTime;
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, useMagicObject.token);
		}
		// エフェクト再生
		UniTask task = EffectManager.Instance.PlayEffect(eEffectType.Elimination, miniBullet.position);
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
