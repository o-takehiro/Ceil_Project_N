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
using static PlayerMagicAttack;
using static CommonModule;
using static UnityEngine.EventSystems.EventTrigger;
using System.Threading;
using System.Drawing;
using Cysharp.Threading.Tasks.Triggers;

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
	// コピーした魔法
	private List<eMagicType> _copyMagicList = null;
	// 魔法のリセットがすでに呼ばれているかどうか
	private List<List<bool>> _isResetMagic = null;

	// 魔法生成中
	public bool magicGenerate = false;

	// ある程度の生成数
	private const int _MAGIC_MAX = 8;

	public void Initialize() {
		instance = this;

		// 魔法のクラスをある程度生成して未使用状態にしておく
		_useList = new List<MagicBase>(_MAGIC_MAX);

		int sideTypeMax = (int)eSideType.Max - 1;
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
			_unuseObjectList[i].Initialize();
		}

		// 魔法の種類分のリストを生成しておく
		int magicTypeMax = (int)eMagicType.Max;
		_copyMagicList = new List<eMagicType>(magicTypeMax);

		// 発動中の魔法リストを魔法の種類分生成
		_activeMagicIDList = new List<List<int>>(sideTypeMax);
		for (int i = 0; i < sideTypeMax; i++) {
			_activeMagicIDList.Add(new List<int>(magicTypeMax));
			for (int magicCount = 0; magicCount < magicTypeMax; magicCount++) {
				// 未使用状態にしておく
				_activeMagicIDList[i].Add(-1);
			}
		}

		// 発動する魔法リストをある程度生成
		_activeMagic = new List<List<Action<MagicObject>>>(sideTypeMax);
		for (int i = 0; i < sideTypeMax; i++) {
			_activeMagic.Add(new List<Action<MagicObject>>(magicTypeMax));
			for (int magicCount = 0; magicCount < magicTypeMax; magicCount++) {
				// 未使用状態にしておく
				_activeMagic[i].Add(null);
			}
		}

        // 魔法のリセット呼ばれてるかリストを魔法の種類分生成
        _isResetMagic = new List<List<bool>>(sideTypeMax);
        for (int i = 0; i < sideTypeMax; i++) {
            _isResetMagic.Add(new List<bool>(magicTypeMax));
            for (int magicCount = 0; magicCount < magicTypeMax; magicCount++) {
                // 未使用状態にしておく
                _isResetMagic[i].Add(false);
            }
        }
    }

	public void Update() {
		UniTask task;
		// デバッグ用
		if (Input.GetKey(KeyCode.Z)) task = CreateMagic(eSideType.PlayerSide, eMagicType.Defense);
		if (Input.GetKey(KeyCode.X)) task = CreateMagic(eSideType.PlayerSide, eMagicType.MiniBullet);
		if (Input.GetKey(KeyCode.C)) task = CreateMagic(eSideType.EnemySide, eMagicType.Defense);
		if (Input.GetKey(KeyCode.V)) task = CreateMagic(eSideType.EnemySide, eMagicType.MiniBullet);
		if (Input.GetKey(KeyCode.N)) task = CreateMagic(eSideType.PlayerSide, eMagicType.SatelliteOrbital);
		if (Input.GetKey(KeyCode.M)) task = CreateMagic(eSideType.PlayerSide, eMagicType.LaserBeam);
		if (Input.GetKey(KeyCode.L)) task = CreateMagic(eSideType.PlayerSide, eMagicType.DelayBullet);
		if (Input.GetKey(KeyCode.K)) task = CreateMagic(eSideType.PlayerSide, eMagicType.Healing);
		if (Input.GetKey(KeyCode.J)) task = CreateMagic(eSideType.PlayerSide, eMagicType.Buff);
		if (Input.GetKey(KeyCode.H)) task = CreateMagic(eSideType.PlayerSide, eMagicType.GroundShock);
		if (Input.GetKey(KeyCode.G)) task = CreateMagic(eSideType.PlayerSide, eMagicType.BigBullet);
		if (Input.GetKeyUp(KeyCode.Z)) task = MagicReset(eSideType.PlayerSide, eMagicType.Defense);
		if (Input.GetKeyUp(KeyCode.X)) task = MagicReset(eSideType.PlayerSide, eMagicType.MiniBullet);
		if (Input.GetKeyUp(KeyCode.C)) task = MagicReset(eSideType.EnemySide, eMagicType.Defense);
		if (Input.GetKeyUp(KeyCode.V)) task = MagicReset(eSideType.EnemySide, eMagicType.MiniBullet);
		if (Input.GetKeyUp(KeyCode.N)) task = MagicReset(eSideType.PlayerSide, eMagicType.SatelliteOrbital);
		if (Input.GetKeyUp(KeyCode.M)) task = MagicReset(eSideType.PlayerSide, eMagicType.LaserBeam);
		if (Input.GetKeyUp(KeyCode.L)) task = MagicReset(eSideType.PlayerSide, eMagicType.DelayBullet);
		if (Input.GetKeyUp(KeyCode.K)) task = MagicReset(eSideType.PlayerSide, eMagicType.Healing);
		if (Input.GetKeyUp(KeyCode.J)) task = MagicReset(eSideType.PlayerSide, eMagicType.Buff);
		if (Input.GetKeyUp(KeyCode.H)) task = MagicReset(eSideType.PlayerSide, eMagicType.GroundShock);
		if (Input.GetKeyUp(KeyCode.G)) task = MagicReset(eSideType.PlayerSide, eMagicType.BigBullet);
		if (Input.GetKeyDown(KeyCode.B)) AnalysisMagicActivate();
		if (Input.GetKeyDown(KeyCode.Q)) ToPlayerDamage(10000);
		if (Input.GetKeyDown(KeyCode.E)) ToEnemyDamage(10000);
		if (Input.GetKeyDown(KeyCode.R)) ExecuteAllMagic(magic => magic.UnuseSelf());

		if (_activeMagic == null) return;

		for (int sideCount = 0; sideCount < (int)eSideType.Max - 1; sideCount++) {
			for (int i = 0, max = _activeMagicIDList[sideCount].Count; i < max; i++) {
				if (_activeMagic[sideCount][i] == null || _activeMagicIDList[sideCount][i] < 0) continue;
				_activeMagic[sideCount][i](GetMagicObject(_activeMagicIDList[sideCount][i]));
			}
		}
		//for (int magic = 0, magicMax = _copyMagicList.Count; magic < magicMax; magic++) {
		//	Debug.Log(_copyMagicList[magic]);
		//}
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
		//Vector3 activePosition = setPosition ?? Vector3.zero;
		int side = (int)sideType, magicID = (int)magicType;
		if (side < 0 || magicID < 0) return;
		if (_activeMagicIDList[side][magicID] >= 0) return;
		magicGenerate = true;
		// データを使用状態にする
		_activeMagicIDList[side][magicID] = UseMagicData(side);
		MagicBase magicSide = GetMagicData(_activeMagicIDList[side][magicID]);
		magicSide?.Setup(_activeMagicIDList[side][magicID], setObject);
		// オブジェクトを生成する
		MagicObject magicObject = GetMagicObject(_activeMagicIDList[side][magicID]);
		if (magicObject == null) {
			//Debug.Log("CreateMagicObject");
			UseMagicObject(_activeMagicIDList[side][magicID], magicType);
			//Debug.Log("CreateComplete");
		}
		// オブジェクト内のオブジェクト生成
		//magicObject.GenerateMiniBullet();
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
		//for (int magic = 0, magicMax = _activeMagic[side].Count; magic < magicMax; magic++) {
		//	if (_activeMagic[side][magic] != null) continue;
		switch (magicType) {
			case eMagicType.Defense:
				_activeMagic[side][magic] = magicSyde.DefenseMagic;
				break;
			case eMagicType.MiniBullet:
				_activeMagic[side][magic] = magicSyde.MiniBulletMagic;
				break;
			case eMagicType.SatelliteOrbital:
				_activeMagic[side][magic] = magicSyde.SatelliteOrbitalMagic;
				break;
			case eMagicType.LaserBeam:
				_activeMagic[side][magic] = magicSyde.LaserBeamMagic;
				break;
            case eMagicType.DelayBullet:
                _activeMagic[side][magic] = magicSyde.DelayBulletMagic;
                break;
			case eMagicType.Healing:
				_activeMagic[side][magic] = magicSyde.HealingMagic;
				break;
			case eMagicType.Buff:
				_activeMagic[side][magic] = magicSyde.BuffMagic;
				break;
			case eMagicType.GroundShock:
				_activeMagic[side][magic] = magicSyde.GroundShockMagic;
				break;
			case eMagicType.BigBullet:
				_activeMagic[side][magic] = magicSyde.BigBulletMagic;
				break;
		}
        //Debug.Log("Action" + magicType);
		// 魔法が生成完了するまで待つ
		while (!magicSyde.useMagicObject.generateFinish) {
			await UniTask.Yield();
		}
        magicGenerate = false;
		return;
		//}
	}

	/// <summary>
	/// 発動中の魔法を終了する
	/// </summary>
	public async UniTask MagicReset(eSideType sideType, eMagicType magicType) {
		int side = (int)sideType, magicID = (int)magicType;
		int activeMagic = _activeMagicIDList[side][magicID];
		MagicBase removeMagic = GetMagicData(activeMagic);
		//activeMagic = -1;
		if (removeMagic == null) return;
		if (removeMagic.ID < 0) return;
		if (_isResetMagic[side][magicID]) return;
		// 魔法が完全に生成されるまで待つ
		while (magicGenerate) {
			await UniTask.Yield();
		}
		// 魔法のリセット
		_activeMagic[side][magicID] = null;
        //Debug.Log("_activeMagicReset");
        _isResetMagic[side][magicID] = true;
		//Debug.Log("0");
        // 未使用化可能まで待つ
        MagicObject magicObject = GetMagicObject(removeMagic.ID);
		while (!magicObject.canUnuse) {
			//Debug.Log(activeMagic + "canUnuse" + magicObject.canUnuse);
			await UniTask.Yield();
		}
		//Debug.Log("1");
		await UnuseMagicData(removeMagic);
		_activeMagicIDList[side][magicID] = -1;
		//Debug.Log("_activeMagicIDList[side][magicID] = activeMagic;");
		_isResetMagic[side][magicID] = false;
    }

	/// <summary>
	/// 魔法を未使用状態にする
	/// </summary>
	/// <param name="unuseMagic"></param>
	public async UniTask UnuseMagicData(MagicBase unuseMagic) {
		//Debug.Log("2");
		if (unuseMagic == null) return;
		//Debug.Log("3");
        // データの未使用化
        int unuseID = unuseMagic.ID;
		if (unuseID < 0) return;
		//Debug.Log("4");
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
		//Debug.Log("5");
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
		UniTask task = EffectManager.Instance.PlayEffect(eEffectType.Analysis, GetEnemyCenterPosition());
	 	_copyMagicList = GetMagicStorageSlot();
		// SE再生
		SoundManager.Instance.PlaySE(5);
		int enemy = (int)eSideType.EnemySide;
		// 発動中の魔法を探す
		for (int magic = 0, magicMax = _activeMagicIDList[enemy].Count; magic < magicMax; magic++) {
			// 魔法発動中かつ、コピー済みでなければセット
			if (!GetMagicActive(enemy, magic) || GetMagicCopied(magic)) continue;
			SetMagicStorageSlot((eMagicType)magic);
			// SE再生
			SoundManager.Instance.PlaySE(17);
			return;
		}
	}

	/// <summary>
	/// 特定の魔法が発動中かどうか
	/// </summary>
	/// <param name="side"></param>
	/// <param name="magic"></param>
	/// <returns></returns>
	public bool GetMagicActive(int side, int magic) {
		return _activeMagicIDList[side][magic] >= 0;
	}

	/// <summary>
	/// 特定の魔法を既にコピーしているかどうか
	/// </summary>
	/// <returns></returns>
	private bool GetMagicCopied(int magicID) { 
		for (int i = 0, max = _copyMagicList.Count; i < max; i++) {
			if ((int)_copyMagicList[i] == magicID) return true;
		}
		return false;
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
