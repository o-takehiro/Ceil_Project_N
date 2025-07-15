/*
 * @file    TutorialEnemy.cs
 * @brief   チュートリアル用の敵
 * @author  Seki
 * @date    2025/7/9
 */
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialEnemy : EnemyCharacter {

    public override void Setup() {
        base.Setup();
        _myAI = new CharacterAIMachine<EnemyCharacter>();
        _myAI.Setup(this);
        _myAI.ChangeState(new EnemyAI002_Move());
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
