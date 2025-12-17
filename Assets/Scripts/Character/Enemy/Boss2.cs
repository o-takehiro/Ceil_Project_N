using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameConst;
using static CharacterUtility;

public class Boss2 : EnemyCharacter {
    public override void Initialize() {
        base.Initialize();
        actionMachine = new EnemyAI009_Boss2Action();
        myAI = new CharacterAIMachine<EnemyCharacter>();
        enemyAnimator = GetComponent<Animator>();
        magicTypeList = new List<eMagicType>(MAX_ENEMY_MAGIC);
    }

    public override void Setup(int masterID) {
        base.Setup(masterID);
        //HPゲージの設定
        SetupCanvasPosition(Vector3.one * 1);
        myAI.Setup(this);
        myAI.ChangeState(new EnemyAI001_Wait());
    }
    public override void Damage(int damage) {
        base.Damage(damage);
        if (isDead || enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("isMagicAttack")) return;

        enemyAnimator.SetTrigger("isDamage");
    }
    private void Update() {
        //現在の位置更新
        SetEnemyPosition(transform.position);
        //現在の回転更新
        SetEnemyRotation(transform.rotation);
        //中心座標更新
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
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
        //座標の更新
        transform.position = GetEnemyPosition();
        transform.rotation = GetEnemyRotation();
        //一フレーム前の位置更新
        SetEnemyPrevPosition();
    }
    public override void Teardown() {
        base.Teardown();
    }
}
