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
using Cysharp.Threading.Tasks;

using static CharacterUtility;
using static CommonModule;

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

    // 魔法のリセットがすでに呼ばれているかどうか
    private List<List<bool>> _isResetMagic = null;

    // 魔法の実行クラス
    private MagicExecutor _executor = null;

	// 魔法生成中
	public bool magicGenerate = false;

	// ある程度の生成数
	private const int _MAGIC_MAX = 8;

	public void Initialize() {
		instance = this;
		_executor = new MagicExecutor();

		// 魔法のクラスをある程度生成して未使用状態にしておく
		_useList = new List<MagicBase>(_MAGIC_MAX);

		int sideTypeMax = (int)eSideType.Max - 1;
		_unuseList = new List<List<MagicBase>>(sideTypeMax);
		for (int i = 0; i < sideTypeMax; i++) {
			_unuseList.Add(new List<MagicBase>(_MAGIC_MAX));
			for (int magicCount = 0; magicCount < _MAGIC_MAX; magicCount++) {
				// 2つの派生クラスを生成してリストに積む
				_unuseList[i].Add(CreateSideMagic((eSideType)i));
				_unuseList[i][magicCount].Initialize();
			}
		}
		// 魔法オブジェクトをある程度生成して未使用状態にしておく
		_useObjectList = new List<MagicObject>(_MAGIC_MAX);

		_unuseObjectList = new List<MagicObject>(_MAGIC_MAX);
		for (int i = 0; i < _MAGIC_MAX; i++) {
			_unuseObjectList.Add(Instantiate(_originObject, _unuseObjectRoot));
			_unuseObjectList[i].Initialize();
		}

		int magicTypeMax = (int)eMagicType.Max;
        // 魔法のリセット呼ばれてるかリストを魔法の種類分生成
        _isResetMagic = new List<List<bool>>(sideTypeMax);
        for (int i = 0; i < sideTypeMax; i++) {
            _isResetMagic.Add(new List<bool>(magicTypeMax));
            for (int magicCount = 0; magicCount < magicTypeMax; magicCount++) {
                // 未使用状態にしておく
                _isResetMagic[i].Add(false);
            }
        }

        // 実行クラスの初期化
        _executor.Initialize(sideTypeMax);
    }

	public void LateUpdate() {
		// 魔法実行
		_executor.MagicExecute();
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
		magicData.SetMagicObject(useObject);
		useObject.Setup(useID, magicData.GetSide(), magic);
		return useObject;
	}

	/// <summary>
	/// 魔法生成
	/// </summary>
	/// <param name="magicID"></param>
	public async UniTask CreateMagic(eSideType sideType, eMagicType magicType, GameObject setObject = null) {
		while (magicGenerate) {
			await UniTask.Yield();
		}
		int side = (int)sideType, magic = (int)magicType;
		int activeMagicID = _executor.GetActiveMagicID(side, magic);
		if (side < 0 || magic < 0) return;
		if (activeMagicID >= 0) return;
		magicGenerate = true;
		// データを使用状態にする
		activeMagicID = UseMagicData(side);
		MagicBase magicSide = GetMagicData(activeMagicID);
		magicSide?.Setup(activeMagicID, setObject);
		// オブジェクトを生成する
		MagicObject magicObject = GetMagicObject(activeMagicID);
		if (magicObject == null) {
			UseMagicObject(activeMagicID, magicType);
		}
		// 魔法実行
		UniTask task = MagicActivate(magicSide, sideType, magicType);

		return;

	}

	/// <summary>
	/// 指定された魔法の関数を実行する
	/// </summary>
	/// <param name="magic"></param>
	private async UniTask MagicActivate(MagicBase magicSyde, eSideType sideType, eMagicType magicType) {
		int side = (int)sideType, magic = (int)magicType;
		// 指定された魔法関数を保存
		_executor.SetActiveMagic(side, magic, magicSyde.magicActionList[magicType]);

        // 魔法が生成完了するまで待つ
		while (!magicSyde.useMagicObject.generateFinish) {
			await UniTask.Yield();
		}
        magicGenerate = false;
		return;
	}

	/// <summary>
	/// 発動中の魔法を終了する
	/// </summary>
	public async UniTask MagicReset(eSideType sideType, eMagicType magicType) {
		int side = (int)sideType, magicID = (int)magicType;
		int activeMagicID = _executor.GetActiveMagicID(side, magicID);
		MagicBase removeMagic = GetMagicData(activeMagicID);
		if (removeMagic == null) return;
		if (removeMagic.ID < 0) return;
		if (_isResetMagic[side][magicID]) return;
		// 魔法が完全に生成されるまで待つ
		while (magicGenerate) {
			await UniTask.Yield();
		}
		// 魔法のリセット
		_executor.SetActiveMagic(side, magicID, null);
        _isResetMagic[side][magicID] = true;
		// 未使用化可能まで待つ
        MagicObject magicObject = GetMagicObject(removeMagic.ID);
		while (!magicObject.canUnuse) {
			await UniTask.Yield();
		}
		await UnuseMagicData(removeMagic);
		// 一番最後にリセット
		_executor.SetActiveMagicID(side, magicID, -1);
		_isResetMagic[side][magicID] = false;
    }

	/// <summary>
	/// 魔法を未使用状態にする
	/// </summary>
	/// <param name="unuseMagic"></param>
	public async UniTask UnuseMagicData(MagicBase unuseMagic) {
		if (unuseMagic == null) return;
		// データの未使用化
        int unuseID = unuseMagic.ID;
		if (unuseID < 0) return;
		_useList[unuseID] = null;
		unuseMagic.Teardown();
		_unuseList[(int)unuseMagic.GetSide()].Add(unuseMagic);
		// オブジェクトの未使用化
		await UnuseMagicObject(GetMagicObject(unuseID));
	}

	/// <summary>
	/// 魔法オブジェクトを未使用状態にする
	/// </summary>
	/// <param name="unuseObject"></param>
	public async UniTask UnuseMagicObject(MagicObject unuseObject) {
		if (unuseObject == null) return;
		if (unuseObject.ID < 0) return;
		// 未使用状態にする
		_useObjectList[unuseObject.ID] = null;
		unuseObject.Teardown();
		_unuseObjectList.Add(unuseObject);
		unuseObject.transform.SetParent(_unuseObjectRoot);
		await UniTask.CompletedTask;
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
	/// 解析魔法の発動
	/// </summary>
	public void AnalysisMagicActivate() {
		if (GetEnemy() == null) return;
		// 解析魔法実行
		_executor.AnalysisMagicExecute();
	}

	/// <summary>
	/// 特定の魔法が発動中かどうか
	/// </summary>
	/// <param name="side"></param>
	/// <param name="magic"></param>
	/// <returns></returns>
	public bool GetMagicActive(int side, int magic) {
		return _executor.GetActiveMagicID(side, magic) >= 0;
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
