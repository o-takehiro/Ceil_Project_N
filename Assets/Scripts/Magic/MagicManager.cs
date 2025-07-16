/*
 * @file    MagicManager.cs
 * @brief   魔法管理クラス
 * @author  Riku
 * @date    2025/7/9
 */

using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEditor.Search;
using UnityEngine;
using System;

using static CommonModule;
using System.Drawing;

public class MagicManager : MonoBehaviour {
	// 自身への参照
	public static MagicManager instance { get; private set; } = null;

	// 使用中魔法オブジェクトの親オブジェクト
	[SerializeField]
	private Transform _useObjectRoot = null;
	// 未使用魔法オブジェクトの親オブジェクト
	[SerializeField]
	private Transform _unuseObjectRoot = null;
	// 魔法オブジェクトのオリジナル
	[SerializeField]
	private MagicObject _originObject = null;

	// 使用中の魔法リスト
	private List<MagicBase> _useList = null;
	// 未使用状態の魔法リスト
	private List<List<MagicBase>> _unuseList = null;

	// 使用中の魔法オブジェクト
	private List<MagicObject> _useObjectList = null;
	// 未使用状態の魔法オブジェクト
	private List<MagicObject> _unuseObjectList = null;

	// 発動する魔法
	private Action activeMagic = null;

	// 発動中の敵の魔法ID
	public eMagicType activeEnemyMagicID = eMagicType.Invalid;
	// コピーした魔法ID
	public List<eMagicType> copyMagicIDList = null;

	private const int _MAGIC_MAX = 10;

	public void Initialize() {
		instance = this;

		// 魔法のクラスをある程度生成して未使用状態にしておく
		_useList = new List<MagicBase>(_MAGIC_MAX);

		int sideTypeMax = (int)eSideType.Max;
		_unuseList = new List<List<MagicBase>>(sideTypeMax);
		for (int i = 0; i < sideTypeMax; i++) {
			_unuseList.Add(new List<MagicBase>(_MAGIC_MAX));
			for (int magicCount = 0; magicCount < _MAGIC_MAX; magicCount++) {
				// 2つの派生クラスを生成してリストに積む
				_unuseList[i].Add(CreateSideMagic((eSideType)i));
			}
		}
		// 魔法オブジェクトをある程度生成して未使用状態にしておく
		_useObjectList = new List<MagicObject>(_MAGIC_MAX);

		_unuseObjectList = new List<MagicObject>(_MAGIC_MAX);
		for (int i = 0; i < _MAGIC_MAX; i++) {
			_unuseObjectList.Add(Instantiate(_originObject, _unuseObjectRoot));
		}

		// 魔法の種類分のリストを生成しておく
		int magicTypeMax = (int)eMagicType.Max;
		copyMagicIDList = new List<eMagicType>(magicTypeMax);
	}

	public void Update() {
		if (Input.GetKey(KeyCode.Z)) MagicUtility.CreateMagic(eSideType.PlayerSide, eMagicType.Defense);
		if (Input.GetKey(KeyCode.X)) MagicUtility.CreateMagic(eSideType.PlayerSide, eMagicType.MiniBullet);
		if (Input.GetKey(KeyCode.C)) MagicUtility.CreateMagic(eSideType.EnemySide, eMagicType.Defense);
		if (Input.GetKey(KeyCode.V)) MagicUtility.CreateMagic(eSideType.EnemySide, eMagicType.MiniBullet);

		if (activeMagic == null) return;
		activeMagic();
	}

	/// <summary>
	/// 魔法を使う陣営に応じたクラスのインスタンスを返す
	/// </summary>
	/// <param name="side"></param>
	/// <returns></returns>
	private MagicBase CreateSideMagic(eSideType side) {
		switch (side) {
			case eSideType.PlayerSide:
				return new PlayerMagic();
			case eSideType.EnemySide:
				return new EnemyMagic();
		}
		return null;
	}

	/// <summary>
	/// ID指定の魔法オブジェクト取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public MagicObject GetMagicObject(int ID) {
		if (!IsEnableIndex(_useObjectList, ID)) return null;

		return _useObjectList[ID];
	}

	/// <summary>
	/// ID指定の魔法データ取得
	/// </summary>
	/// <param name="ID"></param>
	/// <returns></returns>
	public MagicBase GetMagicData(int ID) {
		if (!IsEnableIndex(_useList, ID)) return null;

		return _useList[ID];
	}

	/// <summary>
	/// 魔法を使用状態にする
	/// </summary>
	/// <param name="magicSideIndex"></param>
	/// <returns></returns>
	private int UseMagicData(int magicSideIndex) {
		// 使用可能な魔法データのインスタンス取得
		MagicBase useMagic = GetUsableMagicData(magicSideIndex);
		// 使用可能なIDを取得して使用リストに追加
		int useID = -1;
		for (int i = 0, max = _useList.Count; i < max; i++) {
			if (_useList[i] != null) continue;
			// 使用可能な場所が見つかった
			useID = i;
			_useList[i] = useMagic;
			break;
		}
		// リストに使用可能な場所が見つからなかったので末尾に追加
		if (useID < 0) {
			useID = _useList.Count;
			_useList.Add(useMagic);
		}
		return useID;
	}

	/// <summary>
	/// 使用可能な魔法データのインスタンスを返す
	/// </summary>
	/// <param name="sideIndex"></param>
	/// <returns></returns>
	private MagicBase GetUsableMagicData(int sideIndex) {
		// 未使用状態のインスタンスがあれば返す、無ければ生成して返す
		List<MagicBase> targetList = _unuseList[sideIndex];
		if (IsEmpty(targetList)) return CreateSideMagic((eSideType)sideIndex);

		MagicBase result = targetList[0];
		targetList.RemoveAt(0);
		return result;
	}

	/// <summary>
	/// 魔法オブジェクトを使用状態にする
	/// </summary>
	/// <param name="useID"></param>
	/// <returns></returns>
	public MagicObject UseMagicObject(int useID) {
		// 使用可能な魔法オブジェクトのインスタンスを取得
		MagicObject useObject = GetUsableMagicObject();
		// useIDが有効になるように使用リストの要素を追加する
		while (!IsEnableIndex(_useObjectList, useID)) _useObjectList.Add(null);
		// 使用リストへの追加
		_useObjectList[useID] = useObject;
		useObject.transform.SetParent(_useObjectRoot);
		MagicBase magicData = GetMagicData(useID);
		useObject.Setup(useID, magicData.GetSide());
		return useObject;
	}

	/// <summary>
	/// 魔法生成
	/// </summary>
	/// <param name="magicID"></param>
	public void	CreateMagic(eSideType side, eMagicType magicID) {
		// データを使用状態にする
		int useID = UseMagicData((int)side);
		MagicBase magicSide = GetMagicData(useID);
		magicSide?.Setup(useID);
		// 魔法実行
		MagicActivate(magicSide, magicID);
	}

	/// <summary>
	/// 魔法を未使用状態にする
	/// </summary>
	/// <param name="unuseMagic"></param>
	public void UnuseMagic(MagicBase unuseMagic) {
		if (unuseMagic == null) return;
		// データの未使用化
		int unuseID = unuseMagic.ID;
		_useList[unuseMagic.ID] = null;
		unuseMagic.Teardown();
		_unuseList[(int)unuseMagic.GetSide()].Add(unuseMagic);
		// オブジェクトの未使用化
		UnuseMagicObject(GetMagicObject(unuseID));
	}

	/// <summary>
	/// 魔法オブジェクトを未使用状態にする
	/// </summary>
	/// <param name="unuseObject"></param>
	public void UnuseMagicObject(MagicObject unuseObject) {
		if (unuseObject == null) return;
		// 未使用状態にする
		_useObjectList[unuseObject.ID] = null;
		unuseObject.Teardown();
		_unuseObjectList.Add(unuseObject);
		unuseObject.transform.SetParent(_useObjectRoot);
	}

	/// <summary>
	/// 未使用状態の魔法オブジェクト取得
	/// </summary>
	/// <returns></returns>
	private MagicObject GetUsableMagicObject() {
		if (IsEmpty(_unuseObjectList)) return Instantiate(_originObject);

		MagicObject result = _unuseObjectList[0];
		_unuseObjectList.RemoveAt(0);
		return result;
	}

	/// <summary>
	/// 指定された魔法の関数を実行する
	/// </summary>
	/// <param name="magic"></param>
	private void MagicActivate(MagicBase sideData, eMagicType magic) {
		switch (magic) {
			case eMagicType.Defense:
				activeMagic = sideData.DefenseMagic;
				break;
			case eMagicType.MiniBullet:
				activeMagic = sideData.MiniBulletMagic;
				break;
		}
	}

	/// <summary>
	/// 解析魔法の発動
	/// </summary>
	public void AnalysisMagicActivate() {
		// 敵の発動中の魔法を取得
		for (int i = 0, max = copyMagicIDList.Count; i < max; i++) {
			if (copyMagicIDList[i] == activeEnemyMagicID) return;
		}
		copyMagicIDList.Add(activeEnemyMagicID);
	}

	/// <summary>
	/// 全ての魔法に指定処理実行
	/// </summary>
	/// <param name="action"></param>
	public void ExecuteAllMagic(Action<MagicBase> action) {
		if (action == null || IsEmpty(_useList)) return;

		for (int i = 0, max = _useList.Count; i < max; i++) {
			if (_useList[i] == null) continue;

			action(_useList[i]);
		}
	}
}
