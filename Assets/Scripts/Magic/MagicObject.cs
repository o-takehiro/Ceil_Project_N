/*
 * @file    MagicObject.cs
 * @brief   魔法オブジェクトクラス
 * @author  Riku
 * @date    2025/7/9
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
using UnityEngine;

public class MagicObject : MonoBehaviour {
	// ユニークのID
	public int ID { get; private set; } = -1;

	// 防御魔法オブジェクトのオリジナル
	[SerializeField]
	private GameObject _originDefense = null;
	// 小型弾幕オブジェクトのオリジナル
	[SerializeField]
	private GameObject _originMiniBullet = null;
	// ビームオブジェクトのオリジナル
	[SerializeField]
	private GameObject _originBeam = null;
    // バフオブジェクトのオリジナル
    [SerializeField]
    private GameObject _originBuff = null;
    // 衝撃波オブジェクトのオリジナル
    [SerializeField]
    private GameObject _originGroundShock = null;

    // 防御魔法のオブジェクト
    public GameObject defenseObject = null;
    // 小型弾幕のオブジェクト
    public List<GameObject> miniBulletObjects = null;
	// ビーム魔法のオブジェクト
	public GameObject beamObject = null;
    // バフ魔法のオブジェクト
    public GameObject buffObject = null;
    // 衝撃波魔法のオブジェクト
    public GameObject groundShockObject = null;

    // 使用中オブジェクトの親オブジェクト
    [SerializeField]
	private Transform _useObjectRoot = null;
	// 未使用オブジェクトの親オブジェクト
	[SerializeField]
	private Transform _unuseObjectRoot = null;

	// 魔法の実物オブジェクトの未使用親オブジェクト
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

	// タスク中断用トークン
	public CancellationToken token;

	/// <summary>
	/// 初期化
	/// </summary>
	public void Initialize() {
		// 各魔法オブジェクトの準備
		defenseObject = Instantiate(_originDefense, _unuseMagicRoot);
		miniBulletObjects = new List<GameObject>(_GENERATE_OBJECTS_MAX);
		for (int i = 0; i < _GENERATE_OBJECTS_MAX; i++) {
			miniBulletObjects.Add(Instantiate(_originMiniBullet, _unuseMagicRoot));
		}
		beamObject = Instantiate(_originBeam, _unuseMagicRoot);
		buffObject = Instantiate(_originBuff, _unuseMagicRoot);
		groundShockObject = Instantiate(_originGroundShock, _unuseMagicRoot);

		// オブジェクト破棄時に処理されるタスク中断用トークンを取得
		token = this.GetCancellationTokenOnDestroy();
	}

	/// <summary>
	/// 使用前準備
	/// </summary>
	/// <param name="setID"></param>
	/// <param name="side"></param>
	/// <param name="magic"></param>
	public void Setup(int setID, eSideType side, eMagicType magic) {
		canUnuse = false;
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
		GameObject newBullet = Instantiate(_originMiniBullet, magicObjectList[(int)activeMagic]);
		miniBulletObjects.Add(newBullet);
		return newBullet;
	}

	/// <summary>
	/// ビーム魔法の生成
	/// </summary>
	/// <returns></returns>
	public GameObject GenerateBeam() {
		// 非表示のオブジェクトを表示する
		beamObject.transform.SetParent(magicObjectList[(int)activeMagic]);
		beamObject.GetComponent<MagicHit>().Setup(this);

		return beamObject;
	}

	/// <summary>
	/// バフ魔法の生成
	/// </summary>
	/// <returns></returns>
	public GameObject GenerateBuff() {
		// 非表示のオブジェクトを表示する
		buffObject.transform.SetParent(magicObjectList[(int)activeMagic]);
		buffObject.GetComponent<MagicHit>().Setup(this);

		return buffObject;
	}

	/// <summary>
	/// 衝撃波魔法の生成
	/// </summary>
	/// <returns></returns>
	public GameObject GenerateGroundShock() {
		// 非表示のオブジェクトを表示する
		groundShockObject.transform.SetParent(magicObjectList[(int)activeMagic]);
		groundShockObject.GetComponent<MagicHit>().Setup(this);

		return groundShockObject;
	}

	/// <summary>
	/// 発動中の魔法の親オブジェクト取得
	/// </summary>
	/// <returns></returns>
	public Transform GetActiveMagicParent() {
		return magicObjectList[(int)activeMagic];
	}

	/// <summary>
	/// 特定の小型弾幕の削除
	/// </summary>
	/// <param name="removeObject"></param>
	public void RemoveMiniBullet(GameObject gameObject) {
		Transform removeObject = gameObject.transform;
		removeObject.position = Vector3.zero;
		removeObject.rotation = Quaternion.identity;
		removeObject.localScale = Vector3.one;
		removeObject.SetParent(_unuseMagicRoot);
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
		gameObject.transform.position = Vector3.zero;
		gameObject.transform.rotation = Quaternion.identity;
		UnuseMagic();
	}
}