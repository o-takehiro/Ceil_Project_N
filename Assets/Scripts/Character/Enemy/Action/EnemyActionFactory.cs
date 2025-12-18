using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EnemyActionFactory {
    // 敵行動リスト
    private static readonly Dictionary<eEnemyActionType, Func<IEnemyAction>> factoryList =
        new Dictionary<eEnemyActionType, Func<IEnemyAction>>();

    /// <summary>
    /// 初期化処理
    /// </summary>
    public static void Initialize() {
        Register<EnemyAction_Wait>(eEnemyActionType.Wait);
        Register<EnemyAction_CloseMove>(eEnemyActionType.CloseMove);
        Register<EnemyAction_LeaveMove>(eEnemyActionType.LeaveMove);
        Register<EnemyAction_Freeze>(eEnemyActionType.Freeze);

        Register<EnemyAction_NormalAttack>(eEnemyActionType.NormalAttack);
        Register<TutorialBoss_MiniBullet>(eEnemyActionType.MiniBullet);
        Register<TutorialBoss_MagicDefense>(eEnemyActionType.MagicDefense);
        Register<Boss1_ChargeAttack>(eEnemyActionType.ChargeAttack);
        Register<Boss1_GroundShock>(eEnemyActionType.GroundShock);
        Register<Boss2_BeamAttack>(eEnemyActionType.BeamAttack);
        Register<Boss3_RoarAttack>(eEnemyActionType.RoarAttack);
        //Register<Boss3_ShoulderAttack>(eEnemyActionID.OverheadAttack);
    }
    /// <summary>
    /// ファクトリーへ登録
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="setAction"></param>
    public static void Register<T>(eEnemyActionType setAction) where T : IEnemyAction, new() {
        factoryList[setAction] = ()=> new T();
    }
    /// <summary>
    /// 行動リストの生成
    /// </summary>
    /// <param name="setAction"></param>
    /// <returns></returns>
    public static IEnemyAction Create(eEnemyActionType setAction) {
        if (factoryList.TryGetValue(setAction, out var creator)) {
            return creator();
        }
        return null;
    }
}
