/*
 * @file    EffectManager.cs
 * @brief   エフェクトオブジェクトクラス
 * @author  Riku
 * @date    2025/9/9
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

public class EffectObject : MonoBehaviour {
	// エフェクトオブジェクトのオリジナルリスト
	[SerializeField]
	private List<GameObject> _originEffectList = null;

	// 使用中のエフェクトのオブジェクトリスト
	private List<List<GameObject>> _useEffectObjectsList = null;
	// 未使用のエフェクトのオブジェクトリスト
	private List<List<GameObject>> _unuseEffectObjectsList = null;

	// 使用中オブジェクトの親オブジェクト
	[SerializeField]
	private Transform _useObjectRoot = null;
	// 未使用オブジェクトの親オブジェクト
	[SerializeField]
	private Transform _unuseObjectRoot = null;

	// エフェクトのオブジェクトの生成数
	public const int GENERATE_OBJECTS_MAX = 16;

	/// <summary>
	/// 初期化
	/// </summary>
	public void Initialize() {
		// 各魔法オブジェクトの準備
		_unuseEffectObjectsList = new List<List<GameObject>>((int)eEffectType.max);
		for (int i = 0, max = (int)eEffectType.max; i < max; i++) {
			_unuseEffectObjectsList.Add(new List<GameObject>(GENERATE_OBJECTS_MAX));
			for (int obectNum = 0; obectNum < GENERATE_OBJECTS_MAX; obectNum++) {
				_unuseEffectObjectsList[i].Add(Instantiate(_originEffectList[i], _unuseObjectRoot));
			}
		}
		_useEffectObjectsList = new List<List<GameObject>>((int)eEffectType.max);
		for (int i = 0, max = (int)eEffectType.max; i < max; i++) {
			_useEffectObjectsList.Add(new List<GameObject>(GENERATE_OBJECTS_MAX));
		}
	}

	/// <summary>
	/// エフェクトを使用状態にする
	/// </summary>
	/// <param name="effect"></param>
	/// <returns></returns>
	public GameObject UseEffect(eEffectType effect) {
		// 使用可能なエフェクトの取得
		GameObject useObject = GetUsableEffectObject(effect);
		_useEffectObjectsList[(int)effect].Add(useObject);
		useObject.transform.parent = _useObjectRoot;
		return useObject;
	}

	/// <summary>
	/// 未使用状態のエフェクト取得
	/// </summary>
	/// <param name="effectType"></param>
	/// <returns></returns>
	private GameObject GetUsableEffectObject(eEffectType effectType) {
		int effect = (int)effectType;
		if (IsEmpty(_unuseEffectObjectsList[effect])) return Instantiate(_originEffectList[effect]);

		GameObject result = _unuseEffectObjectsList[effect][0];
		_unuseEffectObjectsList[effect].RemoveAt(0);
		return result;
	}

	/// <summary>
	/// エフェクトを未使用状態にする
	/// </summary>
	/// <param name="effect"></param>
	/// <returns></returns>
	public void UnuseEffect(int effect, int number) {
		GameObject result = _useEffectObjectsList[effect][number];
		_unuseEffectObjectsList[effect].Add(result);
		_useEffectObjectsList[effect].RemoveAt(number);
		result.transform.SetParent(_unuseObjectRoot);

	}
}
