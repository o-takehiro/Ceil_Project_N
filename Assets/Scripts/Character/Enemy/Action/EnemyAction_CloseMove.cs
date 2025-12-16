using UnityEngine;

using static EnemyCommonModule;
using static CharacterUtility;

public class EnemyAction_CloseMove : IEnemyAction {
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
        Rigidbody rigidbody = enemy.GetComponent<Rigidbody>();
        if (rigidbody == null) return;

        // プレイヤー方向へ向く
        LookAtPlayer();

        Vector3 dir = (GetPlayerPosition() - enemy.transform.position).normalized;
        dir.y = 0.0f;

        rigidbody.velocity = dir * enemy.GetMoveSpeed();

        enemy.GetEnemyAnimator().SetBool("isMove", true);
    }
    /// <summary>
    /// 片付け処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Teardown(EnemyCharacter enemy) {

    }
}
