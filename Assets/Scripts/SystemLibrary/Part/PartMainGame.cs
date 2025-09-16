using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
using static MagicUtility;
/// <summary>
/// メインゲームパート
/// </summary>
public class PartMainGame : PartBase {

    [SerializeField]
    private CharacterManager _characterManager = null;
    [SerializeField]
    private MagicManager _magicManager = null;
    [SerializeField]
    private StageManager _stageManager = null;

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        // 全ての初期化処理を呼ぶ
        await base.Initialize();
        // キャラクター管理クラス初期化
        _characterManager?.Initialize();
        // 魔法管理クラスの初期化
        _magicManager?.Initialize();
        // ステージ管理クラスの初期化
        _stageManager?.Initialize();
        await MenuManager.Instance.Get<EnemyHPGauge>("Prefabs/Menu/CanvasEnemyUI").Initialize();
        await MenuManager.Instance.Get<PlayerHPGauge>("Prefabs/Menu/CanvasPlayerUI").Initialize();
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// パートの準備
    /// </summary>
    /// <returns></returns>
    public override async UniTask SetUp() {
        await base.SetUp();
        // 生成関連をここに。
        //await FadeManager.Instance.FadeIn();
        //UseEnemy(eEnemyType.TutorialEnemy);
        //UsePlayer(0);
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// パートの実行
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public override async UniTask Execute() {
        await FadeManager.Instance.FadeIn();

        //SoundManager.Instance.PlayBGM(1);
        await MenuManager.Instance.Get<PlayerHPGauge>().Open();

        await UniTask.CompletedTask;

    }

    /// <summary>
    /// パートの片付け
    /// </summary>
    /// <returns></returns>
    public override async UniTask Teardown() {
        await base.Teardown();
        await MenuManager.Instance.Get<PlayerHPGauge>().Close();
        //SoundManager.Instance.PlayBGM(0);
        //UnusePlayer();
        ExecuteAllMagic(magic => magic.UnuseSelf());

        await UniTask.CompletedTask;

    }

}
