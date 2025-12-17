using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static MagicUtility;

public class TutorialBoss_MagicDefense : IEnemyAction {
    // 終了フラグ
    private bool _isFinished = false;

    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy) {
        _isFinished = false;
        CreateMagic(eSideType.EnemySide, eMagicType.Defense);
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
        MagicReset(eSideType.EnemySide, eMagicType.Defense);
    }
    /// <summary>
    /// 終了判定
    /// </summary>
    public bool IsFinished() {
        return _isFinished;
    }
}
