using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactorMasterUtility{
    /// <summary>
    /// ID指定の行動要因マスター取得
    /// </summary>
    /// <param name="enemyID"></param>
    /// <returns></returns>
    public static Entity_EnemyFactorData.Param GetFactorMaster(int enemyID) {
        //ファクターマスターデータ取得
        var factorMasterList = MasterDataManager.enemyFactorData[0];
        //IDが一致するものを返す
        for (int i = 0, max = factorMasterList.Count; i < max; i++) {
            if (factorMasterList[i].EnemyID != enemyID) continue;

            return factorMasterList[i];
        }
        return null;
    }
}
