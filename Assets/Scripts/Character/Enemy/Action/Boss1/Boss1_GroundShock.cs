using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1_GroundShock : IEnemyAction, IEnemyEndAnimation {
    // 終了フラグ
    private bool _isFinished = false;

    private const string _ANIMATION_NAME = "isStompAttack";

    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy) {
        _isFinished = false;
        enemy.GetEnemyAnimator().SetTrigger(_ANIMATION_NAME);
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

    }
    /// <summary>
    /// 終了判定
    /// </summary>
    /// <returns></returns>
    public bool IsFinished() {
        return _isFinished;
    }
    /// <summary>
    /// アニメーション終了処理
    /// </summary>
    public void EndAnimation() {
        _isFinished = true;
    }
}
