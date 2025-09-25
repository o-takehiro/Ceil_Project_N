using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using TMPro;
/// <summary>
/// 取得した魔法を表示するUI
/// </summary>
public class SetMagicUI : MenuBase {

    public static SetMagicUI Instance { get; private set; } = null;

    [SerializeField] private TextMeshProUGUI r1Text;    // R1ボタンテキスト
    [SerializeField] private TextMeshProUGUI r2Text;    // R2ボタンテキスト
    [SerializeField] private TextMeshProUGUI l1Text;    // L1ボタンテキスト
    [SerializeField] private TextMeshProUGUI l2Text;    // L2ボタンテキスト

    [SerializeField] private UnityEngine.UI.Image magicImage;
    [SerializeField]
    public CancellationToken _token;

    public override async UniTask Initialize() {
        Instance = this;
        await UniTask.CompletedTask;
    }

    public override async UniTask Open() {
        _token = this.GetCancellationTokenOnDestroy();

        await base.Open();

        magicImage.gameObject.SetActive(true);

        var tcs = new UniTaskCompletionSource();

        // ステージクリアを監視
        StageManager.Instance.OnStageClear += () => tcs.TrySetResult();

        // ゲームオーバー監視
        async UniTaskVoid WatchGameOver(CancellationToken token) {
            while (!MageAnimationEvents.isGameOver) {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
            // 死亡したらUIをリセット
            ResetMagicUI();
            tcs.TrySetResult();
        }
        WatchGameOver(_token).Forget();

        // どちらかで完了
        await tcs.Task.AttachExternalCancellation(_token);

        magicImage.gameObject.SetActive(false);

        await Close();
    }

    /// <summary>
    /// 魔法リストをUIに反映
    /// </summary>
    public void UpdateMagicUI() {
        List<eMagicType> magicList = PlayerMagicAttack.GetMagicStorageSlot();

        // UI用の配列にまとめる
        TextMeshProUGUI[] slotTexts = { r1Text, r2Text, l1Text, l2Text };

        for (int i = 0; i < slotTexts.Length; i++) {
            if (i < magicList.Count && magicList[i] != eMagicType.Invalid) {
                slotTexts[i].text = magicList[i].ToString();
            }
            else {
                slotTexts[i].text = "None";
            }
        }
    }

    /// <summary>
    /// 文字列リセット用
    /// </summary>
    public void ResetMagicUI() {
        TextMeshProUGUI[] slotTexts = { r1Text, r2Text, l1Text, l2Text };
        foreach (var text in slotTexts) {
            text.text = "None";
        }
    }

    /// <summary>
    /// 魔法リストUIの表示
    /// </summary>
    public void OpenUI() {
        magicImage.gameObject.SetActive(true);
    }

    /// <summary>
    /// 魔法リストUIの非表示
    /// 魔法リストUIの非表示
    /// 魔法リストUIの非表示
    /// </summary>
    public void CloseUI() {
        magicImage.gameObject.SetActive(false);
    }


}
