using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PressButtonAlpha : MonoBehaviour {
    [SerializeField]
    private Image _fadeImage = null;

    private bool _switchFade = false;
    private bool _isClose = false;

    private const float _DEFAULT_FADE_DURATION = 1.0f;
    private CancellationToken _token;
    public void Setup(Color setColor) {
        _fadeImage.color = setColor;
        _isClose = false;
    }

    public async UniTask Execute() {
        _token = this.GetCancellationTokenOnDestroy();
        while (!_isClose) {
            if (_switchFade) {
                await FadeOut();
            } else {
                await FadeIn();
            }
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _token);
        }
    }

    public void CloseMenu() {
        _isClose = true;
    }
    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask FadeOut(float duration = _DEFAULT_FADE_DURATION) {
        _switchFade = false;
        await FadeTargetAlpha(1.0f, duration);
    }
    /// <summary>
    /// フェードイン
    /// </summary>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask FadeIn(float duration = _DEFAULT_FADE_DURATION) {
        _switchFade = true;
        await FadeTargetAlpha(0.0f, duration);
    }
    /// <summary>
    /// フェード画像を指定の不透明度に変化
    /// </summary>
    /// <param name="fadeState"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    private async UniTask FadeTargetAlpha(float targetAlpha, float duration) {
        _token = this.GetCancellationTokenOnDestroy();
        _fadeImage.gameObject.SetActive(true);
        float elapsedTime = 0.0f;
        float startAlpha = _fadeImage.color.a;
        Color targetColor = _fadeImage.color;
        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            // 補完した不透明度をフェード画像に設定
            float t = elapsedTime / duration;
            targetColor.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            _fadeImage.color = targetColor;
            // 1フレーム待つ
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _token);
        }
        targetColor.a = targetAlpha;
        _fadeImage.color = targetColor;
        _fadeImage.gameObject.SetActive(false);
    }
}
