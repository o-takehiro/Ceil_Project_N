using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static CharacterUtility;

public class MagicHit : MagicObject {
	// 親オブジェクト
	MagicObject parentObject = null;

	/// <summary>
	/// 使用前準備
	/// </summary>
	public void Setup(MagicObject parent) {
		activeMagic = parent.activeMagic;
		activeSide = parent.activeSide;
		parentObject = parent;
	}

	/// <summary>
	/// 当たっている時
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerStay(Collider other) {
		// 相手の陣営
		eSideType otherSide = GetOtherSide(other);
        if (otherSide == eSideType.Invalid) return;
		// 相手の使った魔法
		eMagicType otherMagic = GetOtherMagic(other, otherSide);
		// 魔法が当たった時の魔法を使った相手
		Vector3 thisPosition = gameObject.transform.position;

		UniTask task;
		// 魔法ごとの当たり判定
		switch (activeMagic) {
			case eMagicType.Defense:
				break;
			case eMagicType.MiniBullet:
				// ダメージを与える
				GiveDamage(otherSide, 5);
				if (otherSide == eSideType.MagicSide) {
					task = EffectManager.Instance.PlayEffect(eEffectType.Elimination, thisPosition);
					SoundManager.Instance.PlaySE(9);
				}
				else {
					task = EffectManager.Instance.PlayEffect(eEffectType.Hit, thisPosition);
					SoundManager.Instance.PlaySE(10);
				}
				// 魔法消滅
				parentObject.RemoveMagic(gameObject);
				break;
			case eMagicType.SatelliteOrbital:
				// ダメージを与える
				GiveDamage(otherSide, 10);
				if (otherSide == eSideType.MagicSide) {
					task = EffectManager.Instance.PlayEffect(eEffectType.Elimination, thisPosition);
					SoundManager.Instance.PlaySE(9);
				}
				else {
					task = EffectManager.Instance.PlayEffect(eEffectType.Hit, thisPosition);
					SoundManager.Instance.PlaySE(10);
				}
				// 魔法消滅
				parentObject.RemoveMagic(gameObject);
				break;
			case eMagicType.LaserBeam:
				if (otherMagic == eMagicType.Defense) return;
				// ダメージを与える
				GiveDamage(otherSide, 1);
				break;
            case eMagicType.DelayBullet:
                // ダメージを与える
                GiveDamage(otherSide, 10);
				// 当たった相手によってエフェクトを変える
                if (otherSide == eSideType.MagicSide) {
					if (otherSide == eSideType.PlayerSide)
						task = EffectManager.Instance.PlayEffect(eEffectType.BigElimination, thisPosition);
					task = EffectManager.Instance.PlayEffect(eEffectType.Elimination, thisPosition);
					SoundManager.Instance.PlaySE(9);
				}
                else {
					if (otherSide == eSideType.PlayerSide)
						task = EffectManager.Instance.PlayEffect(eEffectType.BigHit, thisPosition);
					task = EffectManager.Instance.PlayEffect(eEffectType.Hit, thisPosition);
					SoundManager.Instance.PlaySE(10);
				}
				// 魔法消滅
                parentObject.RemoveMagic(gameObject);
                break;
			case eMagicType.GroundShock:
				if (otherSide == eSideType.PlayerSide) {
					// プレイヤーがジャンプ中なら当たらない
					if (!GroundCheck.IsGrounded) return;
					// ダメージを与える
					GiveDamage(otherSide, 1);
				}
				else if (otherSide == eSideType.EnemySide) {
					// ダメージを与える
					GiveDamage(otherSide, 2);
				}
				break;
			case eMagicType.BigBullet:
				// ダメージを与える
                GiveDamage(otherSide, 20);
                if (otherSide == eSideType.MagicSide) {
					task = EffectManager.Instance.PlayEffect(eEffectType.BigElimination, thisPosition);
					SoundManager.Instance.PlaySE(9);
				}
                else {
					task = EffectManager.Instance.PlayEffect(eEffectType.BigHit, thisPosition);
					SoundManager.Instance.PlaySE(10);
				}
				// 魔法消滅
                parentObject.RemoveMagic(gameObject);
                break;
		}

	}

	/// <summary>
	/// 当たった相手を取得
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	private eSideType GetOtherSide(Collider other) {
        eSideType otherSide = eSideType.Invalid;
		// 当たった相手のタグによってタイプ分け
        if (other.gameObject.tag == "Player") {
            otherSide = eSideType.PlayerSide;
        }
        else if (other.gameObject.tag == "Enemy") {
            otherSide = eSideType.EnemySide;
        }
        else if (other.gameObject.tag == "Magic") {
            otherSide = eSideType.MagicSide;
        }
        // 同陣営どうしは当たり判定をとらない 
        if (otherSide == activeSide) return eSideType.Invalid;
        // 魔法の発動者の陣営が同じなら当たり判定をとらない
		MagicHit otherMagicData = GetMagicHit(otherSide, other);
		if (otherMagicData == null) return otherSide;
        if (otherMagicData.activeSide == activeSide) return eSideType.Invalid;
        return otherSide;
    }

	/// <summary>
	/// 当たった相手のスクリプト
	/// </summary>
	/// <param name="otherSide"></param>
	/// <param name="other"></param>
	/// <returns></returns>
	private MagicHit GetMagicHit(eSideType otherSide, Collider other) {
        if (otherSide != eSideType.MagicSide) return null;
        MagicHit otherMagicData = otherMagicData = other.gameObject.GetComponent<MagicHit>();
        return otherMagicData;
    }

	/// <summary>
	/// 相手オブジェクトの魔法取得
	/// </summary>
	/// <param name="other"></param>
	/// <param name="otherSide"></param>
	/// <returns></returns>
	private eMagicType GetOtherMagic (Collider other, eSideType otherSide) {
		if (otherSide == eSideType.MagicSide) {
			eMagicType otherMagic = other.GetComponent<MagicObject>().activeMagic;
			return otherMagic;
		}
		else {
			return eMagicType.Invalid;
		}
	}

    /// <summary>
    /// ダメージ付与
    /// </summary>
    /// <param name="otherSide"></param>
    /// <param name="setValue"></param>
    private void GiveDamage(eSideType otherSide, int setValue) {
        switch (otherSide) {
            case eSideType.PlayerSide:
				ToPlayerDamage(setValue);
                break;
            case eSideType.EnemySide:
				ToEnemyDamage(setValue);
                break;
        }
    }
}
