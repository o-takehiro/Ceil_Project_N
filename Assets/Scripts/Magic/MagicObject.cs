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
	
	public List<GameObject> miniBulletObjects = null;

	// 魔法用のオブジェクト
	public Transform defense = null;
	public Transform miniBullet = null;

	// 小型弾幕オブジェクトのオリジナル
	public GameObject originMiniBullet = null;

	// 魔法用のオブジェクトの生成数
	private const int _GENERATE_OBJECTS_MAX = 32;

	public void Setup(int setID, eSideType side) {
		miniBulletObjects = new List<GameObject>(_GENERATE_OBJECTS_MAX);
		for (int i = 0, max = _GENERATE_OBJECTS_MAX; i < max; i++) miniBulletObjects.Add(null);
		ID = setID;
	}

	/// <summary>
	/// 小型弾幕の生成
	/// </summary>
	public void GenerateMiniBullet() {
		for (int i = 0, max = miniBulletObjects.Count + 1 ; i < max; i++) {
			if (miniBulletObjects[i] == null) {
				miniBulletObjects[i] = Instantiate(originMiniBullet, miniBullet);
				return;
			}
			else {
				miniBulletObjects.Add(Instantiate(originMiniBullet, miniBullet));
				return;
			}
		}
	}

	/// <summary>
	/// 片付け
	/// </summary>
	public void Teardown() {
		ID = -1;
		//gameObject.SetActive(false);
	}

}
