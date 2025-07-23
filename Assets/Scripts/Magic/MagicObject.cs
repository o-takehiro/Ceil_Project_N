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

	// 加賀田弾幕のオブジェクト
	public List<GameObject> miniBulletObjects = null;

	// 使用中オブジェクトの親オブジェクト
	[SerializeField]
	private Transform _useObjectRoot = null;
	// 未使用オブジェクトの親オブジェクト
	[SerializeField]
	private Transform _unuseObjectRoot = null;

	// 魔法用のオブジェクト
	public Transform defense = null;
	public Transform miniBullet = null;

	// 小型弾幕オブジェクトのオリジナル
	public GameObject originMiniBullet = null;

	// 魔法用のオブジェクトの生成数
	private const int _GENERATE_OBJECTS_MAX = 32;

	public void Setup(int setID, eSideType side, eMagicType magic) {
		miniBulletObjects = new List<GameObject>(_GENERATE_OBJECTS_MAX);
		for (int i = 0, max = _GENERATE_OBJECTS_MAX; i < max; i++) miniBulletObjects.Add(null);
		ID = setID;
		UseMagic(magic);
	}

	/// <summary>
	/// 魔法を使用中にする
	/// </summary>
	/// <param name="magicID"></param>
	public void UseMagic(eMagicType magic) {
		switch (magic) {
			case eMagicType.Defense:
				defense.SetParent(_useObjectRoot);
				return;
			case eMagicType.MiniBullet:
				miniBullet.SetParent(_useObjectRoot);
				return;
		}
	}

	/// <summary>
	/// 魔法を未使用にする
	/// </summary>
	/// <param name="magicID"></param>
	public void UnuseMagic(eMagicType magic) {
		switch (magic) {
			case eMagicType.Defense:
				defense.SetParent(_unuseObjectRoot);
				return;
			case eMagicType.MiniBullet:
				miniBullet.SetParent(_unuseObjectRoot);
				return;
		}
	}

	/// <summary>
	/// 小型弾幕の生成
	/// </summary>
	public GameObject GenerateMiniBullet() {
		// リストの空き枠に生成
		for (int i = 0, max = miniBulletObjects.Count; i < max; i++) {
			if (miniBulletObjects[i] != null) continue;
			miniBulletObjects[i] = Instantiate(originMiniBullet, miniBullet);
			return miniBulletObjects[i];
		}
		// リストに空きがない場合は追加
		GameObject newBullet = Instantiate(originMiniBullet, miniBullet);
		miniBulletObjects.Add(newBullet);
		return newBullet;
	}

	/// <summary>
	/// 片付け
	/// </summary>
	public void Teardown(eMagicType magic) {
		ID = -1;
		UnuseMagic(magic);
	}
}