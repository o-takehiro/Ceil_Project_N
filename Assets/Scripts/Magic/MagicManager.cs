/*
 * @file    MagicManager.cs
 * @brief   魔法管理クラス
 * @author  Riku
 * @date    2025/7/9
 */

using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

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
}
