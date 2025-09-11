using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        CharacterUtility.UsePlayer(0);
        CharacterManager.instance.SetUsePlayerPosition(PlayerStartPos.transform.position);
        Debug.Log("[StartObject] CreatePlayer 呼ばれました");
    }

    private void CreateEnemy() {
        CharacterUtility.UseEnemy(eEnemyType.TutorialEnemy);
        CharacterManager.instance.SetUseEnemyPosition(EnemyStartPos.transform.position);
    }

}
