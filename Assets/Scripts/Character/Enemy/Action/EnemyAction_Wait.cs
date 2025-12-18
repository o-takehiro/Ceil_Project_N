using UnityEngine;

using static EnemyCommonModule;

public class EnemyAction_Wait : IEnemyAction {
    // 就労判定
    private bool _isFinished = false;
    // 経過時間
    private float elapsedTime = 0.0f; 

    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy) {
        if(!enemy) return;

        _isFinished = false;
        elapsedTime = 0.0f;
        enemy.SetCoolTime();
    }
    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        if(!enemy) return;

        elapsedTime += Time.deltaTime;
        // プレイヤー方向へ向く
        LookAtPlayer(enemy);
        // クールタイム判定
        if(elapsedTime > enemy.GetCoolTime()) {
            _isFinished = true;
            return;
        }
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
}
