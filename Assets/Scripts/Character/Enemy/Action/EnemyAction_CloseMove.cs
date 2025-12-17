using UnityEngine;

using static EnemyCommonModule;
using static CharacterUtility;

public class EnemyAction_CloseMove : IEnemyAction {
    // 終了フラグ
    private bool _isFinished = false;
    // Rigidbodyコンポーネント
    private Rigidbody _rigidbody = null;

    private const string _ANIMATION_NAME = "isMove";

    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy) {
        _isFinished = false;
        _rigidbody = enemy.GetComponent<Rigidbody>();
        enemy.GetEnemyAnimator().SetBool(_ANIMATION_NAME, true);
    }
    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        if (!_rigidbody) return;

        // プレイヤー方向へ向く
        LookAtPlayer();

        Vector3 dir = (GetPlayerPosition() - enemy.transform.position).normalized;
        dir.y = 0.0f;

        _rigidbody.velocity = dir * enemy.GetMoveSpeed();
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
