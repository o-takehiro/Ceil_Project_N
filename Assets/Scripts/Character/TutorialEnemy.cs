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
    }
    private void Update() {
        //AIマシーンの更新
        _myAI.Update();
    }
}
