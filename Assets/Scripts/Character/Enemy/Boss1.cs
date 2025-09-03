using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class Boss1 : EnemyCharacter {
    private const float _CANVAS_POS_Y = 6f;

    public override void Initialize() {
        base.Initialize();
        _actionMachine = new EnemyAI005_Boss1Action();
        _myAI = new CharacterAIMachine<EnemyCharacter>();
    }
    public override void Setup() {
        base.Setup();
        //現在の位置更新
        SetEnemyPosition(transform.position);
        //一フレーム前の位置更新
        SetEnemyPrevPosition();
        //現在の回転更新
        SetEnemyRotation(transform.rotation);
        //HPゲージの更新
        SetupCanvasPosition(_CANVAS_POS_Y, Vector3.one * 2);
        _myAI.Setup(this);
        _myAI.ChangeState(_actionMachine);
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
