/*
 *  @file   PlayerHPGaouge
 *  @author oorui
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPGauge : MenuBase {

    [SerializeField]
    private Slider _hpSlider = null;

    public CancellationToken _token;

    /// <summary>
    /// HPのスライーを取得
    /// </summary>
    /// <returns></returns>
    public Slider GetSlider() => _hpSlider;



    public override async UniTask Open() {
        _token = this.GetCancellationTokenOnDestroy();

        await base.Open();

        _hpSlider.value = 1.0f;

        var tcs = new UniTaskCompletionSource();

        // ステージクリア監視
        StageManager.Instance.OnStageClear += () => tcs.TrySetResult();

        // ゲームオーバー監視
        async UniTaskVoid WatchGameOver(CancellationToken token) {
            while (!MageAnimationEvents.isGameOver) {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
            tcs.TrySetResult();
        }
        WatchGameOver(_token).Forget();

        // どちらかで完了
        await tcs.Task.AttachExternalCancellation(_token);

        await Close();
    }

}
