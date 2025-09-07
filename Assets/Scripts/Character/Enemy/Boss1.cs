using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
using static CharacterMasterUtility;

public class Boss1 : EnemyCharacter {
    private const float _CANVAS_POS_Y = 5.5f;

    public override void Initialize() {
        base.Initialize();
        _actionMachine = new EnemyAI005_Boss1Action();
        _myAI = new CharacterAIMachine<EnemyCharacter>();
        enemyAnimator = GetComponent<Animator>();
    }
    public override void Setup(int masterID) {
        base.Setup(masterID);
        var masterData = GetCharacterMaster(masterID);
        SetMaxHP(masterData.HP);
        SetHP(masterData.HP);
        SetRawAttack(masterData.Attack);
        SetRawDefense(masterData.Defense);
        //現在の位置更新
        SetEnemyPosition(transform.position);
        //一フレーム前の位置更新
        SetEnemyPrevPosition();
        //現在の回転更新
        SetEnemyRotation(transform.rotation);
        //HPゲージの更新
        SetupCanvasPosition(_CANVAS_POS_Y, Vector3.one * 3);
        _myAI.Setup(this);
        _myAI.ChangeState(new EnemyAI001_Wait());
    }
    private void Update() {
        //現在の位置更新
        SetPosition(transform.position);
        //現在の回転取得
        SetRotation(transform.rotation);
        //AIマシーンの更新
        _myAI.Update();
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
}
