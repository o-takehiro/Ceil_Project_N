using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialBoss_MiniBullet : IEnemyAction, IEnemyEndAnimation{
    // 終了フラグ
    private bool _isFinished = false;

    private const string _ANIMATION_NAME = "isMagicAttack";
    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy) {
        _isFinished = false;
        enemy.enemyAnimator.SetTrigger(_ANIMATION_NAME);
    }
    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        // プレイヤーの方向へ向く
        EnemyCommonModule.LookAtPlayer(enemy);
    }
    /// <summary>
    /// 片付け処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Teardown(EnemyCharacter enemy) {
        MagicUtility.MagicReset(eSideType.EnemySide, eMagicType.MiniBullet);
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
    /// <returns></returns>
    public void EndAnimation() {
        _isFinished = true;
    }
}