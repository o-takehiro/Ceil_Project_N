using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// タイトルメニュー
/// </summary>
public class MenuTitle : MenuBase {
    [SerializeField]
    private PressButtonAlpha _pressButton = null;

    public override async UniTask Open() {
        await base.Open();
        // フェード時の色を設定する。
        var fadeColor = FadeType.White;
        Color setColor = Color.white;
        setColor.a = 0;
        _pressButton.Setup(setColor);
        await FadeManager.Instance.FadeIn(fadeColor);
        await _pressButton.FadeIn();
        UniTask task = _pressButton.Execute();
        while (true) {
            if (Input.anyKey) break;

            await UniTask.Delay(1);
        }
        await FadeManager.Instance.FadeOut(fadeColor);
        SoundManager.Instance.StopBGM();
        _pressButton.CloseMenu();
        await Close();
    }
}
