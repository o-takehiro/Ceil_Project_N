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
    [SerializeField] private goalObject _goalObject;
    private bool _isTutorial = false;
    private int _currentStageBGM = -1;

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
        await MenuManager.Instance.Get<PlayerMPGauge>("Prefabs/Menu/CanvasPlayerUI_MP").Initialize();
        await MenuManager.Instance.Get<SetMagicUI>("Prefabs/Menu/CanvasMagicUI").Initialize();
        await MenuManager.Instance.Get<MenuTutorialGuide>("Prefabs/Menu/CanvasTutorialImage").Initialize();
        _isTutorial = true;
        _currentStageBGM = 1;
        await UniTask.CompletedTask;
    }


    /// <summary>
    /// パートの準備
    /// </summary>
    /// <returns></returns>
    public override async UniTask SetUp() {
        await base.SetUp();
        MageAnimationEvents.isGameOver = false;
        _goalObject = GameObject.FindObjectOfType<goalObject>();

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// パートの実行
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public override async UniTask Execute() {

        await FadeManager.Instance.FadeIn();
        SoundManager.Instance.PlayBGM(_currentStageBGM);
        if (_isTutorial) {
            _isTutorial = false;
            await MenuManager.Instance.Get<MenuTutorialGuide>().Open();
        }
        GetEnemy().StartEnemyState();
        ResumePlayer();
        await UniTask.WhenAll(
                MenuManager.Instance.Get<PlayerHPGauge>().Open(),
                MenuManager.Instance.Get<PlayerMPGauge>().Open(),
                MenuManager.Instance.Get<SetMagicUI>().Open()
            );

        await WaitForGameEnd();              // フラグ監視ループ
        await HandleGameEndOrTransition();   // 遷移処理

    }

    /// <summary>
    /// パートの片付け
    /// </summary>
    /// <returns></returns>
    public override async UniTask Teardown() {
        await base.Teardown();
        await MenuManager.Instance.Get<PlayerHPGauge>().Close();
        await MenuManager.Instance.Get<PlayerMPGauge>().Close();
        await MenuManager.Instance.Get<SetMagicUI>().Close();
        ExecuteAllMagic(magic => magic.UnuseSelf());

        await UniTask.CompletedTask;

    }



    /// <summary>
    /// ゴール到達or死亡を見る
    /// </summary>
    private async UniTask WaitForGameEnd() {
        while (!_goalObject.IsPlayerReachedGoal && !MageAnimationEvents.isGameOver) {
            await UniTask.DelayFrame(30);
        }
    }

    /// <summary>
    /// 遷移処理
    /// </summary>
    private async UniTask HandleGameEndOrTransition() {
        // 死んだ場合
        if (MageAnimationEvents.isGameOver) {
            _currentStageBGM = 1;
            await PartManager.Instance.TransitionPart(eGamePart.Ending);
            return;
        }

        // ゴールした場合
        eStageState stage = StageManager.Instance.GetCurrentStageState();
        await HandleStageTransition(stage);
        SoundManager.Instance.StopBGM();
    }

    /// <summary>
    /// ステージごとの遷移処理
    /// </summary>
    private async UniTask HandleStageTransition(eStageState stage) {
        switch (stage) {
            case eStageState.Tutorial:
                _currentStageBGM = 2;
                await FadeManager.Instance.FadeOut();
                await StageManager.Instance.TransitionStage(eStageState.Stage1);
                await PartManager.Instance.TransitionPart(eGamePart.MainGame);
                break;
            case eStageState.Stage1:
                _currentStageBGM = 3;
                await FadeManager.Instance.FadeOut();
                await StageManager.Instance.TransitionStage(eStageState.Stage2);
                await PartManager.Instance.TransitionPart(eGamePart.MainGame);
                break;
            case eStageState.Stage2:
                _currentStageBGM = 4;
                await FadeManager.Instance.FadeOut();
                await StageManager.Instance.TransitionStage(eStageState.Stage3);
                await PartManager.Instance.TransitionPart(eGamePart.MainGame);
                break;
            case eStageState.Stage3:
                _currentStageBGM = 1;
                await FadeManager.Instance.FadeOut();
                PlayerMagicReset();
                UnusePlayer();
                MenuManager.Instance.Get<MenuEnding>().SetGameClear(true);
                await PartManager.Instance.TransitionPart(eGamePart.Ending);
                break;
        }
    }

}
