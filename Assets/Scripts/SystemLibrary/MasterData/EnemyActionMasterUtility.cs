using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyActionMasterUtility {
    /// <summary>
    /// ID指定の敵行動マスター取得
    /// </summary>
    /// <param name="actionID"></param>
    /// <returns></returns>
    public static Entity_EnemyActionData.Param GetActionMaster(int actionID) {
        //ファクターマスターデータ取得
        var actionMasterList = MasterDataManager.enemyActionData[0];
        //IDが一致するものを返す
        for (int i = 0, max = actionMasterList.Count; i < max; i++) {
            if (actionMasterList[i].ID != actionID) continue;

            return actionMasterList[i];
        }
        return null;
    }
}
