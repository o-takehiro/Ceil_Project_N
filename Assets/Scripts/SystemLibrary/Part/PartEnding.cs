using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エンディングパート
/// </summary>
public class PartEnding : PartBase {
    /// <summary>
    /// パートの実行
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute() {
        UniTask task = PartManager.Instance.TransitionPart(eGamePart.Title);

        await UniTask.CompletedTask;
    }
}
