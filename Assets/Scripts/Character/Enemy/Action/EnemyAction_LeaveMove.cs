using UnityEngine;

using static EnemyCommonModule;

public class EnemyAction_LeaveMove : IEnemyAction {
    // 終了フラグ
    private bool _isFinished = false;
    // 経過時間
    private float _elapsedTime = -1.0f;
    // Rigidbody
    private Rigidbody _rigidbody = null;
    // 角度(デグリー)
    private float _angle = -1.0f;

    // 角度の範囲(デグリー)
    private const float _RANGE_ANGLE = 30.0f;
    // 移動時間
    private const float _MOVE_TIME_PER = 3.0f;

    private const string _ANIMATION_NAME = "isMove";

    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy) {
        if(!enemy) return;

        _isFinished = false;
        _elapsedTime = 0.0f;
        _rigidbody = enemy.GetComponent<Rigidbody>();
        // 逃げる角度を決める
        Vector3 back = -enemy.transform.forward;
        float baseAngle = Mathf.Atan2(back.z, back.x) * Mathf.Rad2Deg;
        _angle = baseAngle + Random.Range(-_RANGE_ANGLE, _RANGE_ANGLE);
        // アニメーション設定
        enemy.GetEnemyAnimator().SetBool(_ANIMATION_NAME, true);
    }
    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        if (!enemy || !_rigidbody) return;

        _elapsedTime += Time.deltaTime;
        // 角度からベクトルへ変換
        float rad = _angle * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad));
        // 方向ベクトルへ向く
        LookAtDirection(dir, enemy);
        // 移動
        _rigidbody.velocity = dir * enemy.GetMoveSpeed();
        // 終了判断
        if (!enemy.IsPlayerClose() || _elapsedTime > _MOVE_TIME_PER) {
            _elapsedTime = 0.0f;
            _isFinished = true;
            return;
        }
    }
    /// <summary>
    /// 片付け処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Teardown(EnemyCharacter enemy) {
        if(!enemy) return;
        // 移動量を0にする
        _rigidbody.velocity = Vector3.zero;
        // アニメーション設定
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