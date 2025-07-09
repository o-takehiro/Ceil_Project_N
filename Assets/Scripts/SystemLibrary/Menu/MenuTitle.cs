using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトルメニュー
/// </summary>
public class MenuTitle : MenuBase {
    public override async UniTask Open() {
        await base.Open();
        await FadeManager.Instance.FadeIn(FadeType.White);
        // Spaceキーが押されるまで待つ
        while (true) {
            if (Input.GetKeyDown(KeyCode.Z)) break;

            await UniTask.Delay(1);
        }
        await FadeManager.Instance.FadeOut(FadeType.White);
        await Close();
    }
}
