using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : SystemObject {
    // フェード用黒画像
    [SerializeField]
    private Image _fadeImage = null;

    /// <summary>
    /// 自身への参照
    /// </summary>
    public static FadeManager Instance { get; private set; } = null;

    // デフォルトのフェード時間
    private const float _DEFAULT_FADE_DURATION = 0.1f;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        Instance = this;
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <param name="_duration"></param>
    /// <returns></returns>
    public async UniTask FadeOut(float _duration = _DEFAULT_FADE_DURATION) {
        await FadeTargetAlpha(1.0f, _duration);
    }


    /// <summary>
    /// フェードイン
    /// </summary>
    /// <param name="_duration"></param>
    /// <returns></returns>
    public async UniTask FadeIn(float _duration = _DEFAULT_FADE_DURATION) {
        await FadeTargetAlpha(0.0f, _duration);

    }

    /// <summary>
    /// フェード画像を指定の不透明度に変化させる
    /// </summary>
    /// <param name="_targetAlpha"></param>
    /// <param name="_duration"></param>
    /// <returns></returns>
    private async UniTask FadeTargetAlpha(float _targetAlpha, float _duration) {
        float elapsedTime = 0.0f;   // 経過時間
        float startAlpha = _fadeImage.color.a;
        Color targetColor = _fadeImage.color;
        while (elapsedTime < _duration) {
            // フレーム経過時間
            elapsedTime += Time.deltaTime;
            // 補間した不透明度をフェード画像に設定
            float t = elapsedTime / _duration;
            targetColor.a = Mathf.Lerp(startAlpha, _targetAlpha, t);
            _fadeImage.color = targetColor;
            // 1フレーム待ち
            await UniTask.DelayFrame(5);
        }
        targetColor.a = _targetAlpha;
        _fadeImage.color = targetColor;

    }

}
