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
        //“G‚Ì•ûŒü‚ğŒü‚«‘±‚¯‚é
        Quaternion enemyRotation = GetEnemyRotation();
        //•ûŒü‚ğŒˆ‚ß‚é
        Vector3 direction = GetPlayerPosition() - GetEnemyPosition();
        //…•½•ûŒü‚Ì‚İ‚Ì‰ñ“]‚Ì‚½‚ßAy‚É‚Í0‚ğ‘ã“ü
        direction.y = 0;
        //‰ñ“]‚³‚¹‚é
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        enemyRotation = Quaternion.Lerp(enemyRotation, lookRotation, 0.1f);
        //©g‚Ì‰ñ“]‚É‘ã“ü‚·‚é
        SetEnemyRotation(enemyRotation);
    }

    public override void Teardown() {
        base.Teardown();
    }
}
