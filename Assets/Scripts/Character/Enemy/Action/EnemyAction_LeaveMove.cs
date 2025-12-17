using UnityEngine;

using static EnemyCommonModule;

public class EnemyAction_LeaveMove : IEnemyAction {
    // 終了フラグ
    private bool _isFinished = false;
    // Rigidbody
    private Rigidbody _rigidbody = null;
    // 角度(デグリー)
    private float _angle = -1.0f;

    // 角度の範囲(デグリー)
    private const float _RANGE_ANGLE = 30.0f;
    private const string _ANIMATION_NAME = "isMove";

    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy) {
        _isFinished = false;
        _rigidbody = enemy.GetComponent<Rigidbody>();
        // 逃げる角度を決める
        Vector3 back = -enemy.transform.forward;
        float baseAngle = Mathf.Atan2(back.z, back.x) * Mathf.Rad2Deg;
        _angle = baseAngle + Random.Range(-_RANGE_ANGLE, _RANGE_ANGLE);
        enemy.GetEnemyAnimator().SetBool(_ANIMATION_NAME, true);
    }
    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        if (!_rigidbody) return;

        // 角度からベクトルへ変換
        float rad = _angle * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));

        _rigidbody.velocity = dir * enemy.GetMoveSpeed();

        // 方向ベクトルへ向く
        LookAtDirection(dir, enemy);
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