using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static CharacterUtility;

public class MagicHit : MagicObject {
	// 親オブジェクト
	MagicObject parentObject = null;

	private const float _BEAM_RANGE_MAX = 20;
	private const float _DEFENSE_RADIUS_MAX = 1.65f;

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
		eSideType otherSide = GetOtherSide(other);
        if (otherSide == eSideType.Invalid) return;
		eMagicType otherMagic = GetOtherMagic(other, otherSide);
		Vector3 thisPosition = gameObject.transform.position;
		Vector3 otherPosition = other.transform.position;
		
		UniTask task;
		// 魔法ごとの当たり判定
		switch (activeMagic) {
			case eMagicType.Defense:
				break;
			case eMagicType.MiniBullet:
				GiveDamage(otherSide, 10);
				if (otherSide == eSideType.MagicSide) {
					task = EffectManager.Instance.PlayEffect(eEffectType.Elimination, thisPosition);
				}
				else {
					task = EffectManager.Instance.PlayEffect(eEffectType.Hit, thisPosition);
				}
				parentObject.RemoveMiniBullet(gameObject);
				break;
			case eMagicType.SatelliteOrbital:
				GiveDamage(otherSide, 10);
				if (otherSide == eSideType.MagicSide) {
					// 消滅エフェクト
					task = EffectManager.Instance.PlayEffect(eEffectType.Elimination, thisPosition);
				}
				else {
					// ヒットエフェクト
					task = EffectManager.Instance.PlayEffect(eEffectType.Hit, thisPosition);
				}
				parentObject.RemoveMiniBullet(gameObject);
				break;
			case eMagicType.LaserBeam:
				if (otherMagic != eMagicType.Defense) {
					GiveDamage(otherSide, 1);
				}
				else {
					// 相手から自分までの方向
					Vector3 thisDirection = (thisPosition - otherPosition).normalized;
					// 相手の防御魔法の正面位置
                    Vector3 otherDefenseForwardPos = otherPosition + thisDirection * _DEFENSE_RADIUS_MAX;
					// ビームがディフェンスに当たったまでの長さ
					float beamRange = _BEAM_RANGE_MAX - Vector3.Distance(thisPosition, otherDefenseForwardPos);
					// ビームの長さを調整
					Vector3 beamScale = gameObject.transform.localScale;
					beamScale.z = beamRange / _BEAM_RANGE_MAX;
                    gameObject.transform.localScale = beamScale;
                    
                }
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
        if (other.gameObject.tag == "Player")
        {
            otherSide = eSideType.PlayerSide;
        }
        else if (other.gameObject.tag == "Enemy")
        {
            otherSide = eSideType.EnemySide;
        }
        else if (other.gameObject.tag == "Magic")
        {
            otherSide = eSideType.MagicSide;
        }
        // 同陣営どうしは当たり判定をとらない 
        if (otherSide == activeSide) return eSideType.Invalid;
        MagicHit otherMagic = null;
        if (otherSide == eSideType.MagicSide)
        {
            otherMagic = other.gameObject.GetComponent<MagicHit>();
            // 魔法の発動者の陣営が同じなら当たり判定をとらない
            if (otherMagic.activeSide == activeSide) return eSideType.Invalid;
        }
		return otherSide;
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
