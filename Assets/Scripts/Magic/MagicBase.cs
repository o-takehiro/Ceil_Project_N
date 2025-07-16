/*
 * @file    MagicBase.cs
 * @brief   魔法データの基底
 * @author  Riku
 * @date    2025/7/9
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MagicBase {
	// ユニークのID
	public int ID { get; private set; } = -1;
	// マスターデータのID
	public int masterID { get; private set; } = -1;
	// 魔法陣営取得
	public abstract eSideType GetSide();
	// 実装魔法
	public abstract void DefenseMagic();
	public abstract void MiniBulletMagic();

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

	}

	/// <summary>
	/// 自身を未使用状態にする
	/// </summary>
	public void UnuseSelf() {

	}

}
