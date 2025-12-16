/*
 *  @file   PartEnding
 *  @author oorui
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エンディングパート
/// </summary>
public class PartEnding : PartBase {
    public override async UniTask Initialize() {
        await base.Initialize();
        await MenuManager.Instance.Get<MenuEnding>("Prefabs/Menu/CanvasEnding").Initialize();
    }
    /// <summary>
    /// パートの実行
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute() {
        await MenuManager.Instance.Get<MenuEnding>().Open();
        UniTask task = PartManager.Instance.TransitionPart(eGamePart.Title);

        await UniTask.CompletedTask;
    }
}
