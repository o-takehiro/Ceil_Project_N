using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class Boss1_ChargeAttack : IEnemyAction {
    // 終了判定
    private bool _isFinished = false;
    // Rigidbody
    private Rigidbody _rigidbody = null;
    // 開始位置
    private Vector3 _startPos = Vector3.zero;
    // 目標位置
    private Vector3 _targetPos = Vector3.zero;

    private const string _ANIMATION_NAME = "isCharge";

    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy) {
        _isFinished = false;
        _rigidbody = enemy.GetComponent<Rigidbody>();
        _startPos = enemy.transform.position;
        _targetPos = GetPlayerPosition();
        enemy.GetEnemyAnimator().SetBool(_ANIMATION_NAME, true);
    }
    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        if (!_rigidbody) return;

        // 移動処理
        Vector3 movePos = (_targetPos - _startPos).normalized;
        _rigidbody.velocity = movePos * enemy.GetMoveSpeed();

    }
    /// <summary>
    /// 片付け処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Teardown(EnemyCharacter enemy) {
        enemy.GetEnemyAnimator().SetBool(_ANIMATION_NAME, false);
    }
    /// <summary>
    /// 終了判定
    /// </summary>
    /// <returns></returns>
    public bool IsFinished() {
        return _isFinished;
    }
}
