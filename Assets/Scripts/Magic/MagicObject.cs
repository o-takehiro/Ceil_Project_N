/*
 * @file    MagicObject.cs
 * @brief   魔法オブジェクトクラス
 * @author  Riku
 * @date    2025/7/9
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MagicObject : MonoBehaviour {
	// ユニークのID
	public int ID { get; private set; } = -1;
	// 魔法用のオブジェクト
	public GameObject defense = null;


	public void Setup(int setID, eSideType side) {
		ID = setID;
	}

	/// <summary>
	/// 片付け
	/// </summary>
	public void Teardown() {
		ID = -1;
		//gameObject.SetActive(false);
	}

}
