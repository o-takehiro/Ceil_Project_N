/*
 *  @file   PartTitle
 *  @author oorui
 */
using Cysharp.Threading.Tasks;

/// <summary>
/// タイトルパート
/// </summary>
public class PartTitle : PartBase {

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
        // メニューの初期化
        await MenuManager.Instance.Get<MenuTitle>("Prefabs/Menu/CanvasTitle").Initialize();
    }

    /// <summary>
    /// パートの実行
    /// </summary>
    /// <returns></returns>
    public override async UniTask Execute() {
        SoundManager.Instance.PlayBGM(0);
        // タイトルを表示
        await MenuManager.Instance.Get<MenuTitle>().Open();
        SoundManager.Instance.StopBGM();

        // メインパートへ遷移
        eStageState stageNum = eStageState.Tutorial;

        if (stageNum == eStageState.Max) {
            UniTask task = PartManager.Instance.TransitionPart(eGamePart.Title);
        }
        else {
            await StageManager.Instance.TransitionStage(stageNum);
            UniTask task = PartManager.Instance.TransitionPart(eGamePart.MainGame);

        }

        await UniTask.CompletedTask;

    }
}
