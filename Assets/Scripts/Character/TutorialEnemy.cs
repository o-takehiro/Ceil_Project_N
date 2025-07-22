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

public class TutorialEnemy : EnemyCharacter {
    private const float _CANVAS_POS_Y = 2.0f;
    public override void Setup() {
        base.Setup();
        SetupCanvasPosition(_CANVAS_POS_Y);
        _myAI = new CharacterAIMachine<EnemyCharacter>();
        _myAI.Setup(this);
        _myAI.ChangeState(new EnemyAI001_Wait());
    }
    private void Update() {
        //AIマシーンの更新
        _myAI.Update();
        SetPosition(transform.position);
        transform.position = currentPos;
        prevPos = currentPos;
        transform.rotation = currentRot;
        currentRot = transform.rotation;
    }

    
}
