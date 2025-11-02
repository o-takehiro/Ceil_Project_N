using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicMasterUtility {
    /// <summary>
    /// ID指定のマジックマスター取得
    /// </summary>
    /// <param name="masterID"></param>
    /// <returns></returns>
    public static Entity_MagicData.Param GetMagicMaster(eMagicType masterID) {
        //キャラクターマスターデータ取得
        var magicMasterList = MasterDataManager.magicData[0];
        //IDが一致するものを返す
        for (int i = 0, max = magicMasterList.Count; i < max; i++) {
            if (magicMasterList[i].ID != (int)masterID) continue;

            return magicMasterList[i];
        }
        return null;
    }
}
