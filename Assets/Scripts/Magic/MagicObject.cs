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
	
	private List<GameObject> miniBulletObjects = null;

	// 魔法用のオブジェクト
	public Transform defense = null;
	public Transform miniBullet = null;

	// 小型弾幕オブジェクトのオリジナル
	public GameObject originMiniBullet = null;

	// 魔法用のオブジェクトの生成数
	private const int _GENERATE_OBJECTS_MAX = 32;

	public void Setup(int setID, eSideType side) {
		miniBulletObjects = new List<GameObject>(_GENERATE_OBJECTS_MAX);
		ID = setID;
	}

	/// <summary>
	/// 小型弾幕の生成
	/// </summary>
	public void GenerateMiniBullet() {
		//miniBulletObjects = Instantiate(originMiniBullet, miniBullet);
	}

	/// <summary>
	/// 片付け
	/// </summary>
	public void Teardown() {
		ID = -1;
		//gameObject.SetActive(false);
	}

}
