using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyDecisionMasterUtility;

public class DecisionData {
    public float closePlayerDistance { get; private set; } = -1;    // 許容近距離
    public float farPlayerDisance { get; private set; } = -1;       // 許容遠距離
    public float minCoolTime { get; private set; } = -1;            // 最小クールタイム
    public float maxCoolTime { get; private set; } = -1;            // 最大クールタイム
    public List<int> playerActiveMagic { get; private set; } = null;    // プレイヤーが使用したら反応する魔法IDリスト

    /// <summary>
    /// データの設定
    /// </summary>
    /// <param name="enemyID"></param>
    public void SetupData(int enemyID) {
        var decisionData = GetDecisionMaster(enemyID);
        closePlayerDistance = decisionData.ClosePlayerDistance;
        farPlayerDisance = decisionData.FarPlayerDistance;
        minCoolTime = decisionData.MinCoolTime;
        maxCoolTime = decisionData.MaxCoolTime;
        SetPlayerActiveMagic(decisionData.PlayerActiveMagic);
    }
    /// <summary>
    /// プレイヤーが使用していたら反応する魔法IDリストの設定
    /// -1を除いて入れる
    /// </summary>
    public void SetPlayerActiveMagic(int[] setIDList) {
        for (int i = 0, max = setIDList.Length; i < max; i++) {
            int magicID = setIDList[i];
            if (magicID < 0) continue;

            playerActiveMagic.Add(magicID);
        }
    }
}
