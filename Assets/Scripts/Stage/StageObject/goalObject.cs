using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goalObject : StageObjectBase {
    // 自身のCollider
    private Collider _collider;

    public override void SetUp() {
        base.SetUp();
        if (_collider == null) {
            _collider = GetComponent<Collider>();
        }
        _collider.enabled = false;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    protected override void OnUpdate() {
        var enemy = CharacterUtility.GetEnemy();
        if (enemy != null && enemy.isDead) {
            GetComponent<Collider>().enabled = true;
        }

    }

    private async void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;

        // 現在のステージを取得
        eStageState stage = StageManager.Instance.GetCurrentStageState();
        // ステージ遷移
        switch (stage) {
            case eStageState.Stage3:
                // Stage3 → タイトル
                await PartManager.Instance.TransitionPart(eGamePart.Title);
                break;

            case eStageState.Stage2:
                // Stage2 → Stage3
                await StageManager.Instance.TransitionStage(eStageState.Stage3);
                break;

            case eStageState.Stage1:
                // Stage1 → Stage2
                await StageManager.Instance.TransitionStage(eStageState.Stage2);
                break;

            case eStageState.Tutorial:
                // Tutorial → Stage1
                await FadeManager.Instance.FadeOut();
                // await PartManager.Instance.RetryCurrentPart();
                await StageManager.Instance.TransitionStage(eStageState.Stage1);
                await PartManager.Instance.TransitionPart(eGamePart.MainGame);
                break;
        }
    }


}
