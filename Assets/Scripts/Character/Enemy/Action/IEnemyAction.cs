using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAction {
    /// <summary>
    /// アクション実行処理
    /// </summary>
    /// <param name="enemy"></param>
    void Execute(EnemyCharacter enemy);
}
