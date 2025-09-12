/*
 * @file    MagicBase.cs
 * @brief   魔法データの基底
 * @author  Riku
 * @date    2025/7/9
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static MagicUtility;

public abstract class MagicBase {
	// ユニークのID
	public int ID { get; private set; } = -1;
	// マスターデータのID
	public int masterID { get; private set; } = -1;
	// 使用する魔法オブジェクト
	public MagicObject useMagicObject = null;
	
	// 魔法陣営取得
	public abstract eSideType GetSide();
	// 発動魔法セット
	public abstract void SetMagicObject(MagicObject setObject);
	// 実装魔法
	public abstract void DefenseMagic(MagicObject magicObject);
	public abstract void MiniBulletMagic(MagicObject magicObject);
	public abstract void SatelliteOrbitalMagic(MagicObject magicObject);
	public abstract void LaserBeamMagic(MagicObject magicObject);

	/// <summary>
	/// 準備
	/// </summary>
	/// <param name="setID"></param>
	public void Setup(int setID) {
		ID = setID;
	}

	/// <summary>
	/// 片付け
	/// </summary>
	public void Teardown() {
		ID = -1;
	}

	/// <summary>
	/// 自身を未使用状態にする
	/// </summary>
	public void UnuseSelf() {
		useMagicObject.canUnuse = true;
		MagicReset(useMagicObject.activeSide, useMagicObject.activeMagic);
	}

}
