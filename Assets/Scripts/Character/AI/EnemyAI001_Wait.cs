/*
 * @file    EnemyAI001_Wait.cs
 * @brief   “G‚Ì‘Ò‹@AI
 * @author  Seki
 * @date    2025/7/14
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class EnemyAI001_Wait : CharacterAIBase<EnemyCharacter> {
    public override void Setup() {
        base.Setup();

    }

    public override void Execute() {
        base.Execute();
        //“G‚Ì•ûŒü‚ðŒü‚«‘±‚¯‚é
        //ownerClass.transform.LookAt();

    }

    public override void Teardown() {
        base.Teardown();

    }
}
