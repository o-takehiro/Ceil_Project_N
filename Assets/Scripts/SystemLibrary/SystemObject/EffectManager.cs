using Cysharp.Threading.Tasks;
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

	public override async UniTask Initialize() {
		Instance = this;
		await UniTask.CompletedTask;
	}

	/// <summary>
	/// エフェクト再生
	/// </summary>
	public void PlayEffect() {

	}

	/// <summary>
	/// ループエフェクト再生
	/// </summary>
	public void PlayLoopEffect() {

	}

	/// <summary>
	/// エフェクト停止
	/// </summary>
	public void StopEffect() {

	}
}
