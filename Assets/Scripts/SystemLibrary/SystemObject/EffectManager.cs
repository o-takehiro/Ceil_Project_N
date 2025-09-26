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
		// エフェクト使用化
		GameObject effect = _effectObject.UseEffect(playEffect,playPosition, setParent);
		// SetParent用一時的待ち
		await UniTask.Yield();
		effect.transform.position = playPosition;
		// リストに追加
		_useEffectList[(int)playEffect].Add(effect);
		// エンド待ち
		while (effect.GetComponent<ParticleSystem>().isPlaying) {
			await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _token);
		}
		// エフェクトの終了処理
		effect.transform.position = Vector3.zero;
		// エフェクト非表示
		_useEffectList[(int)playEffect].RemoveAt(0);
		_effectObject.UnuseEffect((int)playEffect, 0);
	}
}
