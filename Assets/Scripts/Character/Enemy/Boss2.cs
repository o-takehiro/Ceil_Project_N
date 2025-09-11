using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameConst;
using static CharacterUtility;
using static CharacterMasterUtility;

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
        var characterMaster = GetCharacterMaster(masterID);
        SetMaxHP(characterMaster.HP);
        SetHP(characterMaster.HP);
        SetRawAttack(characterMaster.Attack);
        SetRawDefense(characterMaster.Defense);
        //現在の位置更新
        SetEnemyPosition(transform.position);
        //一フレーム前の位置更新
        SetEnemyPrevPosition();
        //中心座標更新
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
        //現在の回転更新
        SetEnemyRotation(transform.rotation);
        //HPゲージの設定
        SetupCanvasPosition(Vector3.one * 3);
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
