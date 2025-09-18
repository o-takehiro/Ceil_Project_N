/*
 * @file    EffectManager.cs
 * @brief   エフェクト管理クラス
 * @author  Riku
 * @date    2025/9/9
 */

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
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

	// タスク中断用トークン
	private CancellationToken _token;

	public override async UniTask Initialize() {
		Instance = this;
		// エフェクトオブジェクトの初期化
		_effectObject.Initialize();
		// 使用エフェクトリストを適当数生成
		_useEffectList = new List<List<GameObject>>((int)eEffectType.max);
		for (int i = 0, max = (int)eEffectType.max; i < max; i++) {
			_useEffectList.Add(new List<GameObject>(EffectObject.GENERATE_OBJECTS_MAX));
		}
		// オブジェクト破棄時に処理されるタスク中断用トークンを取得
		_token = this.GetCancellationTokenOnDestroy();

		await UniTask.CompletedTask;
	}

	/// <summary>
	/// エフェクト再生
	/// </summary>
	public async UniTask PlayEffect(eEffectType playEffect, Vector3 playPosition, Transform setParent = null) {
		GameObject effect = null;
		// エフェクト使用化
		effect = _effectObject.UseEffect(playEffect, setParent);
		// SetParent用一時的待ち
		await UniTask.DelayFrame(1);
		effect.transform.position = playPosition;
		// リストに追加する前にエフェクトリストの中身が空か判別
		bool effectListEmpty = GetEffectLisEmpty();
		// リストに追加
		_useEffectList[(int)playEffect].Add(effect);
		// リストに追加する前の段階で空ではなかったらreturn
		if (!effectListEmpty) return;
		// リストが空になるまでエンド待ち
		do {
			// エフェクトの終了処理
			EffectEnd();
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _token);
		} while (!GetEffectLisEmpty());
	}

	/// <summary>
	/// 使用中エフェクトリストの各エフェクトの中身全てが空かどうか
	/// </summary>
	/// <returns></returns>
	private bool GetEffectLisEmpty() {
		for (int i = 0, max = (int)eEffectType.max; i < max; i++) {
			if (!IsEmpty(_useEffectList[i])) return false;
		}
		return true;
	}

	/// <summary>
	/// エフェクトの終了
	/// </summary>
	private void EffectEnd() {
		for (int effect = 0, effectMax = (int)eEffectType.max; effect < effectMax; effect++) {
			for (int i = 0, max = _useEffectList[effect].Count; i < max; i++) {
				if (_useEffectList[effect][i].GetComponent<ParticleSystem>().isPlaying) continue;
				// エフェクト非表示
				_useEffectList[effect].RemoveAt(i);
				_effectObject.UnuseEffect(effect, i);
				break;
			}
		}
	}
}
