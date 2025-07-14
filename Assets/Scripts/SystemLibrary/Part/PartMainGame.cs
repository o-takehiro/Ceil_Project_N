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

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        // 全ての初期化処理を呼ぶ
        await base.Initialize();
        // キャラクター管理クラス初期化
        _characterManager?.Initialize();
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
        UsePlayer();
        UseEnemy(0);
        await UniTask.CompletedTask;
    }



    /// <summary>
    /// パートの実行
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public override async UniTask Execute() {


        await UniTask.CompletedTask;

    }

    /// <summary>
    /// パートの片付け
    /// </summary>
    /// <returns></returns>
    public override async UniTask Teardown() {
        await base.Teardown();
        await UniTask.CompletedTask;

    }

}
