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

	// 小型弾幕のオブジェクト
	public List<GameObject> miniBulletObjects = null;

	// 使用中オブジェクトの親オブジェクト
	[SerializeField]
	private Transform _useObjectRoot = null;
	// 未使用オブジェクトの親オブジェクト
	[SerializeField]
	private Transform _unuseObjectRoot = null;

	// 魔法オブジェクトの未使用親オブジェクト
	[SerializeField]
	private Transform _unuseMiniBulletRoot = null;

	// 魔法用のオブジェクト
	public Transform defense = null;
	public Transform miniBullet = null;

	// 小型弾幕オブジェクトのオリジナル
	public GameObject originMiniBullet = null;

	// 魔法用のオブジェクトの生成数
	private const int _GENERATE_OBJECTS_MAX = 32;

	public void Setup(int setID, eSideType side, eMagicType magic) {
		miniBulletObjects = new List<GameObject>(_GENERATE_OBJECTS_MAX);
		for (int i = 0, max = _GENERATE_OBJECTS_MAX; i < max; i++) {
			miniBulletObjects.Add(Instantiate(originMiniBullet, _unuseMiniBulletRoot));
		}
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
				RemoveMiniBulletAll();
				miniBullet.SetParent(_unuseObjectRoot);
				return;
		}
	}

	/// <summary>
	/// 小型弾幕の生成
	/// </summary>
	public GameObject GenerateMiniBullet() {
		// 非表示のオブジェクトを表示する
		for (int i = 0, max = miniBulletObjects.Count; i < max; i++) {
			if (miniBulletObjects[i].transform.parent == miniBullet) continue;
			miniBulletObjects[i].transform.SetParent(miniBullet);
			return miniBulletObjects[i];
		}
		// 全て表示されている場合は生成
		GameObject newBullet = Instantiate(originMiniBullet, miniBullet);
		miniBulletObjects.Add(newBullet);
		return newBullet;
	}

	/// <summary>
	/// 特定の小型弾幕の削除
	/// </summary>
	/// <param name="removeObject"></param>
	public void RemoveMiniBullet(GameObject removeObject) {
		removeObject.transform.SetParent(_unuseMiniBulletRoot);
	}

	/// <summary>
	/// 全ての小型弾幕の削除
	/// </summary>
	/// <param name="removeObject"></param>
	public void RemoveMiniBulletAll() {
		for (int i = 0, max = miniBulletObjects.Count; i < max; i++) {
			miniBulletObjects[i].transform.SetParent(_unuseMiniBulletRoot);
		}
	}

	/// <summary>
	/// 片付け
	/// </summary>
	public void Teardown(eMagicType magic) {
		ID = -1;
		UnuseMagic(magic);
	}
}