using UnityEngine;

using static EnemyCommonModule;
using static CharacterUtility;

public class EnemyAction_CloseMove : IEnemyAction {
    // 終了フラグ
    private bool _isFinished = false;
    // 経過時間
    private float _elapsedTime = -1.0f;
    // Rigidbodyコンポーネント
    private Rigidbody _rigidbody = null;
    // 移動時間
    private const float _MOVE_TIME_PER = 5.0f;

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
        // アニメーションの設定
        enemy.enemyAnimator.SetBool(_ANIMATION_NAME, true);
    }
    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        if (!enemy || !_rigidbody) return;

        _elapsedTime += Time.deltaTime;
        // 方向ベクトルを求める
        Vector3 dir = (GetPlayerPosition() - enemy.transform.position).normalized;
        dir.y = 0.0f;
        // プレイヤー方向へ向く
        LookAtPlayer();
        // 移動処理
        _rigidbody.velocity = dir * enemy.GetMoveSpeed();
        // 終了判断
        if (!enemy.IsPlayerFar() || _elapsedTime > _MOVE_TIME_PER) {
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
        // アニメーションの設定
        enemy.enemyAnimator.SetBool(_ANIMATION_NAME, false);
    }
    /// <summary>
    /// 終了判定
    /// </summary>
    /// <returns></returns>
    public bool IsFinished() {
        return _isFinished;
    }
}
