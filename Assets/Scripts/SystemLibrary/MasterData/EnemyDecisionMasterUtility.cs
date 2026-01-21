using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDecisionMasterUtility{
    /// <summary>
    /// ID指定の行動要因マスター取得
    /// </summary>
    /// <param name="enemyID"></param>
    /// <returns></returns>
    public static Entity_EnemyDecisionData.Param GetDecisionMaster(int enemyID) {
        //ファクターマスターデータ取得
        var decisionMasterList = MasterDataManager.enemyDecisionData[0];
        //IDが一致するものを返す
        for (int i = 0, max = decisionMasterList.Count; i < max; i++) {
            if (decisionMasterList[i].EnemyID != enemyID) continue;

            return decisionMasterList[i];
        }
        return null;
    }
}
