/*
 * @file    MagicManager.cs
 * @brief   魔法管理クラス
 * @author  Riku
 * @date    2025/7/9
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using static CommonModule;
using Cysharp.Threading.Tasks;

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
	private List<List<Action<MagicObject>>> _activeMagic = null;
	// 発動中の魔法ID
	private List<List<int>> _activeMagicIDList = null;
	//private eMagicType activeEnemyMagicID = eMagicType.Invalid;
	// コピーした魔法ID
	private List<int> _copyMagicIDList = null;

	private const int _MAGIC_MAX = 8;

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
		_copyMagicIDList = new List<int>(magicTypeMax);

		// 発動中の魔法リストをある程度生成
		_activeMagicIDList = new List<List<int>>(sideTypeMax);
		for (int i = 0; i < sideTypeMax; i++) {
			_activeMagicIDList.Add(new List<int>(_MAGIC_MAX));
			for (int magicCount = 0; magicCount < _MAGIC_MAX; magicCount++) {
				// 未使用状態にしておく
				_activeMagicIDList[i].Add(-1);
			}
		}

		// 発動する魔法リストをある程度生成
		_activeMagic = new List<List<Action<MagicObject>>>(sideTypeMax);
		for (int i = 0; i < sideTypeMax; i++) {
			_activeMagic.Add(new List<Action<MagicObject>>(_MAGIC_MAX));
			for (int magicCount = 0; magicCount < _MAGIC_MAX; magicCount++) {
				// 未使用状態にしておく
				_activeMagic[i].Add(null);
			}
		}
	}

	public void Update() {
		// デバッグ用
		if (Input.GetKeyDown(KeyCode.Z)) CreateMagic(eSideType.PlayerSide, eMagicType.Defense);
		if (Input.GetKeyDown(KeyCode.X)) CreateMagic(eSideType.PlayerSide, eMagicType.MiniBullet);
		if (Input.GetKeyDown(KeyCode.C)) CreateMagic(eSideType.EnemySide, eMagicType.Defense);
		if (Input.GetKeyDown(KeyCode.V)) CreateMagic(eSideType.EnemySide, eMagicType.MiniBullet);
		if (Input.GetKeyUp(KeyCode.Z)) MagicReset(eSideType.PlayerSide, eMagicType.Defense);
		if (Input.GetKeyUp(KeyCode.X)) MagicReset(eSideType.PlayerSide, eMagicType.MiniBullet);
		if (Input.GetKeyUp(KeyCode.C)) MagicReset(eSideType.EnemySide, eMagicType.Defense);
		if (Input.GetKeyUp(KeyCode.V)) MagicReset(eSideType.EnemySide, eMagicType.MiniBullet);
		if (Input.GetKeyDown(KeyCode.B)) AnalysisMagicActivate();

		if (_activeMagic == null) return;

		for (int sideCount = 0; sideCount < (int)eSideType.Max; sideCount++) {
			for (int i = 0, max = _activeMagicIDList.Count; i < max; i++) {
				if (_activeMagic[sideCount][i] == null || _activeMagicIDList[sideCount][i] < 0) continue;
				_activeMagic[sideCount][i](GetMagicObject(_activeMagicIDList[sideCount][i]));
			}
		}

		for (int i = 0, max = _copyMagicIDList.Count; i < max; i++) {
			Debug.Log(_copyMagicIDList[i]);
		}
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
	public MagicObject UseMagicObject(int useID, eMagicType magic) {
		// 使用可能な魔法オブジェクトのインスタンスを取得
		MagicObject useObject = GetUsableMagicObject();
		// useIDが有効になるように使用リストの要素を追加する
		while (!IsEnableIndex(_useObjectList, useID)) _useObjectList.Add(null);
		// 使用リストへの追加
		_useObjectList[useID] = useObject;
		useObject.transform.SetParent(_useObjectRoot);
		MagicBase magicData = GetMagicData(useID);
		useObject.Setup(useID, magicData.GetSide(), magic);
		return useObject;
	}

	/// <summary>
	/// 魔法生成
	/// </summary>
	/// <param name="magicID"></param>
	public void CreateMagic(eSideType side, eMagicType magicID) {
		if (_activeMagicIDList[(int)side][(int)magicID] >= 0) return;
		// データを使用状態にする
		_activeMagicIDList[(int)side][(int)magicID] = UseMagicData((int)side);
		MagicBase magicSide = GetMagicData(_activeMagicIDList[(int)side][(int)magicID]);
		magicSide?.Setup(_activeMagicIDList[(int)side][(int)magicID]);
		// オブジェクトを生成する
		MagicObject magicObject = GetMagicObject(_activeMagicIDList[(int)side][(int)magicID]);
		if (magicObject == null) {
			magicObject = UseMagicObject(_activeMagicIDList[(int)side][(int)magicID], magicID);
		}
		// オブジェクト内のオブジェクト生成
		magicObject.GenerateMiniBullet();
		// 魔法実行
		MagicActivate(magicSide, side, magicID);
		return;

	}

	/// <summary>
	/// 指定された魔法の関数を実行する
	/// </summary>
	/// <param name="magic"></param>
	private void MagicActivate(MagicBase magicSyde, eSideType side, eMagicType magic) {
		for (int i = 0, max = _activeMagic[(int)side].Count; i < max; i++) {
			if (_activeMagic[(int)side][i] != null) continue;
			switch (magic) {
				case eMagicType.Defense:
					_activeMagic[(int)side][i] = magicSyde.DefenseMagic;
					break;
				case eMagicType.MiniBullet:
					_activeMagic[(int)side][i] = magicSyde.MiniBulletMagic;
					break;
			}
			return;
		}
	}

	/// <summary>
	/// 発動中の魔法を終了する
	/// </summary>
	public void MagicReset(eSideType sideType, eMagicType magicID) {
		int activeMagic = _activeMagicIDList[(int)sideType][(int)magicID];
		// 魔法のリセット
		_activeMagic[(int)sideType][(int)magicID] = null;
		MagicBase removeMagic = GetMagicData(activeMagic);
		activeMagic = -1;
		_activeMagicIDList[(int)sideType][(int)magicID] = activeMagic;
		UnuseMagicData(removeMagic, magicID);
	}

	/// <summary>
	/// 魔法を未使用状態にする
	/// </summary>
	/// <param name="unuseMagic"></param>
	public void UnuseMagicData(MagicBase unuseMagic, eMagicType magicID) {
		if (unuseMagic == null) return;
		// データの未使用化
		int unuseID = unuseMagic.ID;
		_useList[unuseMagic.ID] = null;
		unuseMagic.Teardown();
		_unuseList[(int)unuseMagic.GetSide()].Add(unuseMagic);
		// オブジェクトの未使用化
		UniTask task = UnuseMagicObject(GetMagicObject(unuseID), magicID);
	}

	/// <summary>
	/// 魔法オブジェクトを未使用状態にする
	/// </summary>
	/// <param name="unuseObject"></param>
	public async UniTask UnuseMagicObject(MagicObject unuseObject, eMagicType magicID) {
		if (unuseObject == null) return;
		// 未使用化可能まで待つ
		while (unuseObject.canUnuse == false) {
			await UniTask.DelayFrame(1);
		}
		// 未使用状態にする
		_useObjectList[unuseObject.ID] = null;
		unuseObject.Teardown(magicID);
		_unuseObjectList.Add(unuseObject);
		unuseObject.transform.SetParent(_unuseObjectRoot);
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
	/// 特定の魔法が発動中かどうか
	/// </summary>
	/// <param name="side"></param>
	/// <param name="magic"></param>
	/// <returns></returns>
	public bool IsMagicActive(eSideType side, eMagicType magic) {
		if (_activeMagicIDList[(int)side][(int)magic] < 0) return true;
		return false;
	}

	/// <summary>
	/// 解析魔法の発動
	/// </summary>
	public void AnalysisMagicActivate() {
		int activeEnemyMagic = -1;
		// 敵の発動中の魔法を取得
		for (int i = 0, max = _copyMagicIDList.Count; i < max; i++) {
			activeEnemyMagic = _activeMagicIDList[(int)eSideType.EnemySide][i];
			if (_copyMagicIDList[i] == activeEnemyMagic) return;
		}
		_copyMagicIDList.Add(activeEnemyMagic);
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
