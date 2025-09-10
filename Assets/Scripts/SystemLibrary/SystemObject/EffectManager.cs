/*
 * @file    EffectManager.cs
 * @brief   エフェクト管理クラス
 * @author  Riku
 * @date    2025/9/9
 */

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

/// <summary>
/// エフェクトの管理
/// </summary>
public class EffectManager : SystemObject {
	/// <summary>
	/// 自身への参照
	/// </summary>
	public static EffectManager Instance { get; private set; } = null;

	// エフェクトオブジェクト
	[SerializeField]
	private EffectObject _effectObject = null;

	// 使用中のエフェクトリスト
	private List<List<GameObject>> _useEffectList = null;

	public override async UniTask Initialize() {
		Instance = this;
		// エフェクトオブジェクトの初期化
		_effectObject.Initialize();
		// 使用エフェクトリストを適当数生成
		_useEffectList = new List<List<GameObject>>((int)eEffectType.max);
		for (int i = 0, max = (int)eEffectType.max; i < max; i++) {
			_useEffectList.Add(new List<GameObject>(EffectObject.GENERATE_OBJECTS_MAX));
		}

		await UniTask.CompletedTask;
	}

	/// <summary>
	/// エフェクト再生
	/// </summary>
	public void PlayEffect(eEffectType playEffect, Vector3 playPosition) {
		GameObject effect = null;
		effect = _effectObject.UseEffect(playEffect);
		effect.transform.position = playPosition;
		_useEffectList[(int)playEffect].Add(effect);
	}

	private void EffectEnd() {
		for (int effect = 0, effectMax = (int)eEffectType.max; effect < effectMax; effect++) {
			for (int i = 0, max = _useEffectList[effect].Count; i < max; i++) {
				if (_useEffectList[effect][i].GetComponent<ParticleSystem>().isPlaying) return;
				// エフェクト非表示
			}
		}
	}
}
