using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

using static CharacterUtility;
using static GameConst;

public class Boss1 : EnemyCharacter {
    private const float _CANVAS_POS_Y = 5.5f;

    public override void Initialize() {
        base.Initialize();
        decision = new Boss1Decision();
    }
    public override void Setup(int masterID) {
        base.Setup(masterID);
        //HPゲージの更新
        SetupCanvasPosition(Vector3.one);
        // アクションの設定
        ChangeAction(eEnemyActionType.Wait);
    }
    private void Update() {
        //現在の位置更新
        SetPosition(transform.position);
        //現在の回転取得
        SetRotation(transform.rotation);
        //中心座標更新
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
        // 敵行動判断リスト
        ExecuteFactors();
        // 行動更新
        ExecuteAction();
        // 行動判断更新処理
        ExecuteDecision();
        //オブジェクトの座標更新
        transform.position = currentPos;
        //オブジェクトの回転更新
        transform.rotation = currentRot;
        //1フレーム前の座標更新
        SetEnemyPrevPosition();
    }
    public override void Teardown() {
        base.Teardown();
    }

    public override void Damage(int damage) {
        base.Damage(damage);
        if (IsDead) return;

        enemyAnimator.SetTrigger("isDamage");
    }
}
