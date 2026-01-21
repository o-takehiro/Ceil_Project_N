using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAction_Freeze : IEnemyAction, IEnemyEndAnimation {
    private bool _isFinished = false;

    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy) {

    }
    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {

    }
    /// <summary>
    /// 片付け処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Teardown(EnemyCharacter enemy) {
        _isFinished = true;
    }
    /// <summary>
    /// 終了判断
    /// </summary>
    /// <returns></returns>
    public bool IsFinished() {
        return _isFinished;
    }
    /// <summary>
    /// 終了処理
    /// </summary>
    public void EndAnimation() {
        _isFinished = true;
    }
}
