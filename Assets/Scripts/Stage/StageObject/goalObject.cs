using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goalObject : StageObjectBase {

    public override void SetUp() {
        base.SetUp();

    }

    protected override void OnUpdate() {
    }

    private void OnTriggerEnter(Collider collision) {
        if (collision.gameObject.tag == "Player") {
            // SE再生

            // 遷移処理
            eStageState stage = eStageState.Max;
            if (stage == eStageState.Stage3) {
                // タイトルに遷移
                UniTask task = PartManager.Instance.TransitionPart(eGamePart.Title);
            }
            else if (stage == eStageState.Stage2) {
                // [ステージ2]のとき[ステージ3]に
                UniTask task = StageManager.Instance.TransitionStage(eStageState.Stage3);
            }
            else if (stage == eStageState.Stage1) {
                // [ステージ1]のとき[ステージ2]に
                UniTask task = StageManager.Instance.TransitionStage(eStageState.Stage2);
            }

        }
    }

}
