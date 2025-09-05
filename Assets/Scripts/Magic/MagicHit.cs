using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	/// 当たった時
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other) {
		eSideType otherSide = eSideType.Invalid;
		if (other.gameObject.tag == "Player") {
			otherSide = eSideType.PlayerSide;
		}
		else if (other.gameObject.tag == "Enemy") {
			otherSide = eSideType.EnemySide;
		}
		// 同陣営どうしは当たり判定をとらない 
		if (otherSide == activeSide) return;
		MagicHit otherMagic = null;
		if (otherSide == eSideType.Invalid) {
			otherMagic = other.gameObject.GetComponent<MagicHit>();
			// 魔法の発動者の陣営が同じなら当たり判定をとらない
			if (otherMagic.activeSide == activeSide) return;
		}

		// 魔法ごとの当たり判定
		switch (activeMagic) {
			case eMagicType.Defense:
				break;
			case eMagicType.MiniBullet:
				parentObject.RemoveMiniBullet(gameObject);
				break;
			case eMagicType.SatelliteOrbital:
				parentObject.RemoveMiniBullet(gameObject);
				break;
		}

	}
}
