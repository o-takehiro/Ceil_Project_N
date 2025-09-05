using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
/// <summary>
/// メインゲームパート
/// </summary>
public class PartMainGame : PartBase {

    [SerializeField]
    private CharacterManager _characterManager = null;
    [SerializeField]
    private MagicManager _magicManager = null;

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
        await FadeManager.Instance.FadeIn();
        UsePlayer(0);
        UseEnemy(0, 1);
        await UniTask.CompletedTask;
    }



    /// <summary>
    /// パートの実行
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public override async UniTask Execute() {
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

        await UniTask.CompletedTask;

    }

}
