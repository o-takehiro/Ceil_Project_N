using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicHit : MagicObject {

	/// <summary>
	/// 使用前準備
	/// </summary>
	public void Setup(eMagicType magic, eSideType side) {
		activeMagic = magic;
		activeSide = side;
	}

	/// <summary>
	/// 当たった時
	/// </summary>
	/// <param name="other"></param>
	private void OnTriggerEnter(Collider other) {
		// 同陣営どうしは当たり判定をとらない 
		eSideType otherSide = eSideType.Invalid;
		if (other.gameObject.tag == "Player") {
			otherSide = eSideType.PlayerSide;
		}
		else if (other.gameObject.tag == "Enemy") {
			otherSide = eSideType.EnemySide;
		}
		if (otherSide == activeSide) return;
		MagicHit otherMagic = null;
		if (otherSide == eSideType.Invalid) {
			otherMagic = other.gameObject.GetComponent<MagicHit>();
			if (otherMagic.activeSide == activeSide) return;
		}

		// 魔法ごとの当たり判定
		switch (activeMagic) {
			case eMagicType.Defense:
				break;
			case eMagicType.MiniBullet:
				if (otherMagic == null) {
					RemoveMiniBullet(gameObject);
					break;
				}
				if (otherMagic.activeMagic != eMagicType.Defense) break;
					RemoveMiniBullet(gameObject);
				break;
		}

	}
}
