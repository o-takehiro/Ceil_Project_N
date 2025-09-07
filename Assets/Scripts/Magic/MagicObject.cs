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

	// 防御魔法オブジェクトのオリジナル
	public GameObject originDefense = null;

	// 小型弾幕オブジェクトのオリジナル
	public GameObject originMiniBullet = null;

	// 防御魔法のオブジェクト
	public GameObject defenseObject = null;

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

	// 魔法用のオブジェクトリスト
	public List<Transform> magicObjectList = new List<Transform>();

	// 発動中の魔法とその陣営
	public eMagicType activeMagic = eMagicType.Invalid;
	public eSideType activeSide = eSideType.Invalid;

	// 未使用化可能かどうか
	public bool canUnuse = true;

	// 魔法用のオブジェクトの生成数
	public const int _GENERATE_OBJECTS_MAX = 16;

	/// <summary>
	/// 初期化
	/// </summary>
	public void Initialize() {
		// 各魔法オブジェクトの準備
		defenseObject = Instantiate(originDefense, _unuseObjectRoot);
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
		magicObjectList[(int)activeMagic].SetParent(_useObjectRoot);
	}

	/// <summary>
	/// 魔法を未使用にする
	/// </summary>
	/// <param name="magicID"></param>
	public void UnuseMagic() {
		magicObjectList[(int)activeMagic].SetParent(_unuseObjectRoot);
	}

	/// <summary>
	/// 防御魔法の生成
	/// </summary>
	/// <returns></returns>
	public GameObject GenerateDefense() {
        // 非表示のオブジェクトを表示する
        defenseObject.transform.SetParent(magicObjectList[(int)activeMagic]);
        defenseObject.GetComponent<MagicHit>().Setup(this);

        return defenseObject;
    }

    /// <summary>
    /// 小型弾幕の生成
    /// </summary>
    public GameObject GenerateMiniBullet() {
		// 非表示のオブジェクトを表示する
		for (int i = 0, max = miniBulletObjects.Count; i < max; i++) {
			if (miniBulletObjects[i].transform.parent == magicObjectList[(int)activeMagic]) continue;
			miniBulletObjects[i].transform.SetParent(magicObjectList[(int)activeMagic]);
			miniBulletObjects[i].GetComponent<MagicHit>().Setup(this);
			
			return miniBulletObjects[i];
		}
		// 全て表示されている場合は生成
		GameObject newBullet = Instantiate(originMiniBullet, magicObjectList[(int)activeMagic]);
		miniBulletObjects.Add(newBullet);
		return newBullet;
	}

	/// <summary>
	/// 特定の小型弾幕の削除
	/// </summary>
	/// <param name="removeObject"></param>
	public void RemoveMiniBullet(GameObject removeObject) {
		removeObject.transform.position = Vector3.zero;
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