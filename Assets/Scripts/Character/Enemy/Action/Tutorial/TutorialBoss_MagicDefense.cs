using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static MagicUtility;

public class TutorialBoss_MagicDefense : IEnemyAction {
    // 終了フラグ
    private bool _isFinished = false;
    // 経過時間
    private float _elapsedTime = -1.0f;

    private const float _WAIT_TIME_PER = 3.0f;

    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy) {
        _isFinished = false;
        _elapsedTime = 0.0f;
        CreateMagic(eSideType.EnemySide, eMagicType.Defense);
    }
    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        // プレイヤーの方向を向く
        EnemyCommonModule.LookAtPlayer(enemy);
        // 一定時間経過したら、このアクションを終了する
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _WAIT_TIME_PER) {
            _isFinished = true;
        }
    }
    /// <summary>
    /// 片付け処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Teardown(EnemyCharacter enemy) {
        MagicReset(eSideType.EnemySide, eMagicType.Defense);
    }
    /// <summary>
    /// 終了判定
    /// </summary>
    public bool IsFinished() {
        return _isFinished;
    }
}
