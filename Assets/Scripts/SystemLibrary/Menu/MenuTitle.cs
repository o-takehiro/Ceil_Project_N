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
        // フェード時の色を設定する。
        var fadeColor = FadeType.White;
        await FadeManager.Instance.FadeIn(fadeColor);
        SoundManager.Instance.PlayBGM(0);
        // 何かが押されるまで待つ
        /*
         * 後にInputSystem対応予定
         */
        while (true) {
            if (Input.anyKey) break;

            await UniTask.Delay(1);
        }
        await FadeManager.Instance.FadeOut(fadeColor);
        SoundManager.Instance.StopBGM();
        await Close();
    }
}
