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

        myAI.Setup(this);
        myAI.ChangeState(new EnemyAI001_Wait());
    }

    private void Update() {
        //現在の位置更新
        SetEnemyPosition(transform.position);
        //現在の回転更新
        SetEnemyRotation(transform.rotation);
        //ステートマシーンの更新
        myAI.Update();
        //座標の更新
        transform.position = GetEnemyPosition();
        transform.rotation = GetEnemyRotation();
        //一フレーム前の位置更新
        SetEnemyPrevPosition();
        //中心座標更新
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
    }
    public override void Teardown() {
        base.Teardown();
    }
}
