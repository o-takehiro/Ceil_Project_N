using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
public class StartObject : StageObjectBase {

    // 出現場所を指定
    [SerializeField]
    private GameObject PlayerStartPos = null;
    [SerializeField]
    private GameObject EnemyStartPos = null;

    public override async void SetUp() {
        base.SetUp();
        CreatePlayer();
        CreateEnemy();
        await UniTask.DelayFrame(1);
    }
    protected override void OnUpdate() {
    }

    /// <summary>
    /// プレイヤーを自身の位置に移動
    /// </summary>
    private void CreatePlayer() {
        UsePlayer(0);
        CharacterManager.instance.SetUsePlayerPosition(PlayerStartPos.transform.position);
    }

    /// <summary>
    /// 敵を自身の位置に移動
    /// </summary>
    private void CreateEnemy() {
        if (GetEnemy() != null) return;
        // 現在のステージを取得
        eStageState stage = StageManager.Instance.GetCurrentStageState();

        switch (stage) {

            case eStageState.Tutorial:
                UseEnemy(eEnemyType.TutorialEnemy);
                break;
            case eStageState.Stage1:
                UseEnemy(eEnemyType.Stage1Enemy);
                break;
            case eStageState.Stage2:
                UseEnemy(eEnemyType.Stage2Enemy);
                break;
            case eStageState.Stage3:
                UseEnemy(eEnemyType.Stage3Enemy);
                break;
        }

        CharacterManager.instance.SetUseEnemyPosition(EnemyStartPos.transform.position);
    }

}
