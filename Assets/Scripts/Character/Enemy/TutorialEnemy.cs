/*
 * @file    TutorialEnemy.cs
 * @brief   チュートリアル用の敵
 * @author  Seki
 * @date    2025/7/9
 */
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static CharacterUtility;
using static CharacterMasterUtility;

public class TutorialEnemy : EnemyCharacter {
    private const float _CANVAS_POS_Y = 4.0f;

    public override void Initialize() {
        base.Initialize();
        _actionMachine = new EnemyAI004_TutorialEnemyAction();
        _actionMachine.Initialize();
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
        SetEnemyPosition(Vector3.zero);
        //一フレーム前の位置更新
        SetEnemyPrevPosition();
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
        //現在の回転更新
        SetEnemyRotation(Quaternion.identity);
        //HPゲージの更新
        SetupCanvasPosition(_CANVAS_POS_Y, transform.position, Vector3.one);
        //ステートマシーンの初期化
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
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
    }
}
