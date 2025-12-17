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
        enemyAnimator = GetComponent<Animator>();
        magicTypeList = new List<eMagicType>(MAX_ENEMY_MAGIC);
    }
    public override void Setup(int masterID) {
        base.Setup(masterID);
        //HPゲージの更新
        SetupCanvasPosition(Vector3.one);
    }
    private void Update() {
        //現在の位置更新
        SetPosition(transform.position);
        //現在の回転取得
        SetRotation(transform.rotation);
        //中心座標更新
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
        // 敵行動判断リスト
        UpdateDecisions();
        // 行動実行
        if (currentAction != null) {
            // 行動実行処理
            currentAction.Execute(this);
            // 終了していたら、クールタイム発動
            if (currentAction.IsFinished())
                factors.isCoolTime = true;
        }
        // 行動中でなければ行動判断をする
        if (!IsAction()) {
            // 行動判断
            eEnemyActionType action = decision.Decide(factors);
            // 行動の変更
            if (action != actionType)
                ChangeAction(action);
        }
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
        if (isDead) return;

        enemyAnimator.SetTrigger("isDamage");
    }
}
