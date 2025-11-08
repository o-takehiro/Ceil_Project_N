/*
 *  @fili   PlayerMPGauge
 *  @author     oorui
 */

using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// プレイヤーのMPゲージ管理
/// </summary>
public class PlayerMPGauge : MenuBase {
    [SerializeField]
    private Slider _mpSlider = null;
    public CancellationToken _token;

    /// <summary>
    /// スライダーの取得
    /// </summary>
    /// <returns></returns>
    public Slider GetSlider() {
        return _mpSlider;
    }


    public override async UniTask Open() {
        _token = this.GetCancellationTokenOnDestroy();

        await base.Open();

        _mpSlider.value = 1.0f;

        var tcs = new UniTaskCompletionSource();

        // ステージクリア監視
        StageManager.Instance.OnStageClear += () => tcs.TrySetResult();

       // ゲームオーバーになるまで待つ
        async UniTaskVoid WatchGameOver(CancellationToken token) {
            while (!MageAnimationEvents.isGameOver) {
                // 1フレームごとにチェック
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
