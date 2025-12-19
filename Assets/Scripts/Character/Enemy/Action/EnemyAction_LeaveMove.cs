using UnityEngine;

using static EnemyCommonModule;

public class EnemyAction_LeaveMove : IEnemyAction {
    // 終了フラグ
    private bool _isFinished = false;
    // 経過時間
    private float _elapsedTime = -1.0f;
    // Rigidbody
    private Rigidbody _rigidbody = null;
    // 現在の移動方向
    private Vector3 _moveDir;

    // 角度の範囲(デグリー)
    private const float _RANGE_ANGLE = 30.0f;
    // 移動時間
    private const float _MOVE_TIME_PER = 3.0f;
    // 壁との距離
    private const float _WALL_CHECK_DISTANCE = 3.0f;
    // 横移動の角度(デグリー)
    private const float _AVOID_ANGLE = 90.0f;
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
        float angle = baseAngle + Random.Range(-_RANGE_ANGLE, _RANGE_ANGLE);
        // 角度をベクトルに変換する
        float rad = angle * Mathf.Deg2Rad;
        _moveDir = new Vector3(Mathf.Cos(rad), 0, Mathf.Sin(rad)).normalized;
        // アニメーション設定
        enemy.enemyAnimator.SetBool(_ANIMATION_NAME, true);
    }
    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        if (!enemy || !_rigidbody) return;

        _elapsedTime += Time.deltaTime;
        // 壁チェック
        if (IsWallAhead(enemy, _moveDir, out RaycastHit hit)) {
            // 反射方向を求める
            Vector3 reflectDir = _moveDir - 2.0f * Vector3.Dot(_moveDir, hit.normal) * hit.normal;
            reflectDir.y = 0.0f;
            reflectDir.Normalize();

            // 反射先もダメなら横逃げ
            if (IsWallAhead(enemy, reflectDir, out _)) {
                reflectDir = GetSideEscapeDir();
            }
            _moveDir = reflectDir;
        }

        // 向き更新
        LookAtDirection(_moveDir, enemy);
        // 移動
        _rigidbody.velocity = _moveDir * enemy.GetMoveSpeed();

        // 終了条件
        if (!enemy.IsPlayerClose() || _elapsedTime > _MOVE_TIME_PER) {
            _isFinished = true;
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
        enemy.enemyAnimator.SetBool(_ANIMATION_NAME, false);
    }
    /// <summary>
    /// 終了判定
    /// </summary>
    /// <returns></returns>
    public bool IsFinished() {
        return _isFinished;
    }
    /// <summary>
    /// 進行方向が壁に衝突するか判定
    /// </summary>
    /// <param name="enemy"></param>
    /// <param name="dir"></param>
    /// <param name="hit"></param>
    /// <returns></returns>
    private bool IsWallAhead(EnemyCharacter enemy, Vector3 dir, out RaycastHit hit) {
        // 足元から少しずらした位置を始点とする
        Vector3 origin = enemy.transform.position + Vector3.up * 0.5f;
        return Physics.Raycast(origin, dir, out hit,_WALL_CHECK_DISTANCE);
    }
    /// <summary>
    /// 横方向のベクトルの取得
    /// </summary>
    /// <returns></returns>
    private Vector3 GetSideEscapeDir() {
        // 右方向か左方向か決める
        float side = Random.value < 0.5f ? _AVOID_ANGLE : -_AVOID_ANGLE;
        Quaternion rot = Quaternion.AngleAxis(side, Vector3.up);
        Vector3 dir = rot * _moveDir;
        dir.y = 0.0f;
        return dir.normalized;
    }
}