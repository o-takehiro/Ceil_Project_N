/*
 * @file    MagicObject.cs
 * @brief   魔法オブジェクトクラス
 * @author  Riku
 * @date    2025/7/9
 */

using Cysharp.Threading.Tasks;
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
	private Transform _unuseMagicRoot = null;

	// 魔法用のオブジェクト
	public Transform defense = null;
	public Transform miniBullet = null;
	public Transform satelliteOrbital = null;

	// 小型弾幕オブジェクトのオリジナル
	public GameObject originMiniBullet = null;

	// 発動中の魔法とその陣営
	protected eMagicType activeMagic = eMagicType.Invalid;
	protected eSideType activeSide = eSideType.Invalid;

	// 未使用化可能かどうか
	public bool canUnuse = true;

	// 魔法用のオブジェクトの生成数
	public const int _GENERATE_OBJECTS_MAX = 16;

	/// <summary>
	/// 初期化
	/// </summary>
	public void Initialize() {
		miniBulletObjects = new List<GameObject>(_GENERATE_OBJECTS_MAX);
		for (int i = 0, max = _GENERATE_OBJECTS_MAX; i < max; i++) {
			miniBulletObjects.Add(Instantiate(originMiniBullet, _unuseMagicRoot));
		}
	}

	/// <summary>
	/// 使用前準備
	/// </summary>
	/// <param name="setID"></param>
	/// <param name="side"></param>
	/// <param name="magic"></param>
	public void Setup(int setID, eSideType side, eMagicType magic) {
		canUnuse = true;
		ID = setID;
		activeSide = side;
		activeMagic = magic;
		UseMagic();
	}

	/// <summary>
	/// 魔法を使用中にする
	/// </summary>
	/// <param name="magicID"></param>
	public void UseMagic() {
		switch (activeMagic) {
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
	public void UnuseMagic() {
		switch (activeMagic) {
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
			miniBulletObjects[i].GetComponent<MagicHit>().Setup(activeMagic, activeSide);
			
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
		removeObject.transform.SetParent(_unuseMagicRoot);
	}

	/// <summary>
	/// 全ての小型弾幕の削除
	/// </summary>
	/// <param name="removeObject"></param>
	public void RemoveMiniBulletAll() {
		for (int i = 0, max = miniBulletObjects.Count; i < max; i++) {
			miniBulletObjects[i].transform.SetParent(_unuseMagicRoot);
		}
	}

	/// <summary>
	/// 片付け
	/// </summary>
	public void Teardown() {
		ID = -1;
		UnuseMagic();
	}
}