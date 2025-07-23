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

public class TutorialEnemy : EnemyCharacter {
    private const float _CANVAS_POS_Y = 2.0f;
    public override void Setup() {
        base.Setup();
        SetEnemyPosition(transform.position);
        SetEnemyRotation(transform.rotation);
        SetupCanvasPosition(_CANVAS_POS_Y);
        _myAI = new CharacterAIMachine<EnemyCharacter>();
        _myAI.Setup(this);
        _myAI.ChangeState(new EnemyAI001_Wait());
    }
    private void Update() {
        //現在の位置更新
        SetCurrentPosition(transform.position);
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
}
