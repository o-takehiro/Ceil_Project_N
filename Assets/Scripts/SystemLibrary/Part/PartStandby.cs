using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 準備パート
/// </summary>
public class PartStandby : PartBase {

    /// <summary>
    /// パートの実行
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute() {
        //マスターデータの読み込み
        MasterDataManager.LoadAllData();
        // フェード時の色
        var fadeColor = FadeType.White;
        // フェードアウト
        await FadeManager.Instance.FadeOut(fadeColor);

        // タイトルパートへ遷移
        UniTask task = PartManager.Instance.TransitionPart(eGamePart.Title);
        await UniTask.CompletedTask;
    }
}
