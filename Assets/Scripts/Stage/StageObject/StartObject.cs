using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartObject : StageObjectBase {

    // 出現場所を指定
    [SerializeField]
    private GameObject PlayerStartPos = null;
    [SerializeField]
    private GameObject EnemyStartPos = null;

    public override void SetUp() {
        base.SetUp();
        CreateEnemy();
        CreatePlayer();
    }
    protected override void OnUpdate() {
    }

    /// <summary>
    /// プレイヤーを自身の位置に移動
    /// </summary>
    private void CreatePlayer() {
        CharacterUtility.UsePlayer(0);
        CharacterManager.instance.SetUsePlayerPosition(PlayerStartPos.transform.position);
    }

    private void CreateEnemy() {
        CharacterUtility.UseEnemy(eEnemyType.TutorialEnemy);
        CharacterManager.instance.SetUseEnemyPosition(EnemyStartPos.transform.position);
    }

}
