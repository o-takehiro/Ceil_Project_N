/*
 * @file    PlayerMagic.cs
 * @brief   プレイヤーの魔法データ
 * @author  Riku
 * @date    2025/7/9
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class PlayerMagic : MagicBase {

	private GameObject activeMagicObject = null;

	/// <summary>
	/// 魔法陣営の取得
	/// </summary>
	/// <returns></returns>
	public override eSideType GetSide() {
		return eSideType.PlayerSide;
	}

	/// <summary>
	/// 解析魔法
	/// </summary>
	public void AnalysisMagic() {

	}
	/// <summary>
	/// 防御魔法
	/// </summary>
	public override void DefenseMagic(MagicObject magicObject) {
		activeMagicObject = magicObject.defense;
		activeMagicObject.transform.position = GetPlayerPosition();
		activeMagicObject.transform.rotation = GetPlayerRotation();

		Debug.Log("PPPPPPPPPPPDDDDDDDDDD");
	}
	/// <summary>
	/// 小型弾幕魔法
	/// </summary>
	public override void MiniBulletMagic(MagicObject magicObject) {
		Debug.Log("PPPPPPPPPPPBBBBBBBBBBBB");
	}
}
