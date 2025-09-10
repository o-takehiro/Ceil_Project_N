using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// パート管理
/// </summary>
public class PartManager : SystemObject {
    /// <summary>
    /// 自身への参照
    /// </summary>
    public static PartManager Instance { get; private set; } = null;


    /// <summary>
    /// パートオブジェクトのオリジナル
    /// </summary>
    [SerializeField]
    private PartBase[] _partOeiginList = null;


    /// <summary>
    /// 管理しているパートオブジェクト
    /// </summary>
    private PartBase[] _partList = null;


    /// <summary>
    /// 現在のパート
    /// </summary>
    private PartBase _currentPart = null;

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        Instance = this;
        // パートオブジェクトの作成
        int partMax = (int)eGamePart.Max;
        _partList = new PartBase[partMax];

        List<UniTask> taskList = new List<UniTask>(partMax);
        for (int i = 0; i < partMax; i++) {
            // パートオブジェクトの生成
            _partList[i] = Instantiate(_partOeiginList[i], transform);
            taskList.Add(_partList[i].Initialize());
        }

        // 全てのパートの初期化処理を待つ
        await CommonModule.WaitTask(taskList);
    }

    /// <summary>
    /// パートの切り替え
    /// </summary>
    /// <param name="_nextPart"></param>
    /// <returns></returns>
    public async UniTask TransitionPart(eGamePart _nextPart) {
        // 現在のパートの片付け
        if (_currentPart != null) await _currentPart.Teardown();
        // パートの切り替え
        _currentPart = _partList[(int)_nextPart];
        await _currentPart.SetUp();
        // 次のパートの実行
        UniTask task = _currentPart.Execute();
    }

    public async UniTask RetryCurrentPart() {
        // 現在のパートの片付け
        if (_currentPart != null) await _currentPart.Teardown();

        await _currentPart.SetUp();
        // 次のパートの実行処理
        UniTask task = _currentPart.Execute();
    }

}
