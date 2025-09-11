using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;
public class StageManager : MonoBehaviour {
    // 自身への参照
    public static StageManager Instance { get; private set; } = null;
    [SerializeField]
    private StageBase[] _stageOriginList = null;
    private StageBase[] _stageList = null;
    // 現在のステージ
    private StageBase _currentStage = null;

    public async UniTask Initialize() {
        Instance = this;
        int stageMax = (int)eStageState.Max;
        _stageList = new StageBase[stageMax];

        List<UniTask> taskList = new List<UniTask>(stageMax);
        for (int i = 0; i < stageMax; i++) {
            _stageList[i] = Instantiate(_stageOriginList[i], transform);
            taskList.Add(_stageList[i].Initialize());
        }
        // 複数のタスクの終了待ちを行う
        await WaitTask(taskList);
    }

    /// <summary>
    /// ステージの遷移
    /// </summary>
    /// <param name="nextStage"></param>
    /// <returns></returns>
    public async UniTask TransitionStage(eStageState nextStage) {
        // 現在のステージの片付け
        if (_currentStage != null) await _currentStage.Teardown();
        // ステージの切り替え
        _currentStage = _stageList[(int)nextStage];
        await _currentStage.SetUp();
        // 次のステージの実行処理
        UniTask task = _currentStage.Execute();
    }

    /// <summary>
    /// リトライ
    /// </summary>
    /// <returns></returns>
    public async UniTask RetryCurrentStage() {
        // 現在のステージの片付け
        if (_currentStage != null) await _currentStage.Teardown();

        await _currentStage.SetUp();
        // 次のステージの実行処理
        UniTask task = _currentStage.Execute();
    }

    public eStageState GetCurrentStageState() {
        if (_currentStage == null) return eStageState.Invalid;
        return _currentStage.StageState;
    }
}
