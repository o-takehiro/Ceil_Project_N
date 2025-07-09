using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static CommonModule;
public class FadeManager : SystemObject {
    public static FadeManager Instance { get; private set; }

    [Header("フェードに使用する画像一覧")]
    [SerializeField]
    private List<Image> _fadeImageList = new();

    private const float _DEFAULT_FADE_DURATION = 0.1f;

    public override async UniTask Initialize() {
        Instance = this;
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <param name="type"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask FadeOut(FadeType type = FadeType.Black, float duration = _DEFAULT_FADE_DURATION) {
        // 画像を取得
        var image = GetImageByType(type);
        SetImageActiveOnly(type);
        if (image != null) await FadeTargetAlpha(image, 1.0f, duration);

    }

    /// <summary>
    /// フェードイン
    /// </summary>
    /// <param name="type"></param>
    /// <param name="duration"></param>
    /// <returns></returns>
    public async UniTask FadeIn(FadeType type = FadeType.Black, float duration = _DEFAULT_FADE_DURATION) {
        // 画像を取得
        var image = GetImageByType(type);
        if (image != null) {
            SetImageActiveOnly(type);
            await FadeTargetAlpha(image, 0.0f, duration);

            // 完全に透明になったら非表示にする
            if (image.color.a <= 0f) {
                image.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// フェード対象の選択
    /// </summary>
    private void SetImageActiveOnly(FadeType activeType) {
        for (int i = 0; i < _fadeImageList.Count; i++) {
            var image = _fadeImageList[i];
            if (image == null) continue;

            image.gameObject.SetActive(i == (int)activeType);
        }
    }

    /// <summary>
    /// フェード画像を指定の不透明度に変化させる
    /// </summary>
    private async UniTask FadeTargetAlpha(Image image, float targetAlpha, float duration) {
        float elapsedTime = 0f;
        float startAlpha = image.color.a;
        Color color = image.color;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            color.a = Mathf.Lerp(startAlpha, targetAlpha, t);
            image.color = color;
            await UniTask.DelayFrame(5);
        }

        color.a = targetAlpha;
        image.color = color;
    }

    /// <summary>
    /// enumに対応する画像を取得
    /// </summary>
    private Image GetImageByType(FadeType type) {
        int index = (int)type;
        if (!IsEnableIndex(_fadeImageList, index)) return null;

        return _fadeImageList[index];

    }

}
