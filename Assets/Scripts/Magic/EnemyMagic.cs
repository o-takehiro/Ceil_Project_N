/*
 * @file    EnemyMagic.cs
 * @brief   “G‚Ì–‚–@ƒf[ƒ^
 * @author  Riku
 * @date    2025/7/9
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMagic : MagicBase {
	// ”­“®’†‚Ì–‚–@ID
	private eMagicType _activeMagicID = 0;

	/// <summary>
	/// –‚–@w‰c‚Ìæ“¾
	/// </summary>
	/// <returns></returns>
	public override eSideType GetSide() {
		return eSideType.EnemySide;
	}

	/// <summary>
	/// –hŒä–‚–@
	/// </summary>
	public override void DefenseMagic() {
		_activeMagicID = eMagicType.Defense;
	}
	/// <summary>
	/// ¬Œ^’e–‹–‚–@
	/// </summary>
	public override void MiniBulletMagic() {
		_activeMagicID = eMagicType.Defense;
	}
}
