using UnityEngine;

using static EnemyCommonModule;

public class EnemyAction_LeaveMove : IEnemyAction {
    private float _angle = -1.0f;

    private const float _RANGE_ANGLE = 30.0f;

    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy) {
        // 逃げる角度を決める
        Vector3 back = -enemy.transform.forward;
        float baseAngle = Mathf.Atan2(back.z, back.x) * Mathf.Rad2Deg;
        _angle = baseAngle + Random.Range(-_RANGE_ANGLE, _RANGE_ANGLE);
    }
    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        Rigidbody rigidbody = enemy.GetComponent<Rigidbody>();
        if (rigidbody == null) return;

        float rad = _angle * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));

        rigidbody.velocity = dir * enemy.GetMoveSpeed();

        // 方向ベクトルへ向く
        LookAtDirection(dir, enemy);
        enemy.GetEnemyAnimator().SetBool("isMove", true);
    }
    /// <summary>
    /// 片付け処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Teardown(EnemyCharacter enemy) {

    }
}