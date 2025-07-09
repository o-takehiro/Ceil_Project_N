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
        // タイトルを表示
        await MenuManager.Instance.Get<MenuTitle>().Open();

        // メインパートへ遷移
        UniTask task = PartManager.Instance.TransitionPart(eGamePart.MainGame);
        await UniTask.CompletedTask;

    }
}
