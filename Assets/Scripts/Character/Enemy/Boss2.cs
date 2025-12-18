using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2 : EnemyCharacter {
    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void Initialize() {
        base.Initialize();
        decision = new Boss2Decision();
    }
    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="masterID"></param>
    public override void Setup(int masterID) {
        base.Setup(masterID);
        //HPゲージの設定
        SetupCanvasPosition(Vector3.one * 1);
        // アクションの設定
        ChangeAction(eEnemyActionType.Wait);
    }
    public override void Damage(int damage) {
        base.Damage(damage);
        if (IsDead || enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("isMagicAttack")) return;

        enemyAnimator.SetTrigger("isDamage");
    }
    private void Update() {
        //現在の位置更新
        SetPosition(transform.position);
        //現在の回転更新
        SetRotation(transform.rotation);
        //中心座標更新
        SetCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
        // 敵行動判断リスト
        ExecuteFactors();
        // 行動更新
        ExecuteAction();
        // 行動判断更新処理
        ExecuteDecision();
        //座標の更新
        transform.position = currentPos;
        transform.rotation = currentRot;
        //一フレーム前の位置更新
        SetPrevPosition();
    }
    public override void Teardown() {
        base.Teardown();
    }
}
