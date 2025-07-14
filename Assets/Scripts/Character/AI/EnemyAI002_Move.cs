using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static CharacterUtility;

public class EnemyAI002_Move : CharacterAIBase<EnemyCharacter> {
    private Rigidbody _enemyRigidbody = null;
    private const float _PLAYER_DISTANCE = 20.0f;
    public override void Setup() {
        base.Setup();
        _enemyRigidbody = ownerClass.GetComponent<Rigidbody>();
    }

    public override void Execute() {
        base.Execute();
        EnemyCloseMove();
    }

    public override void Teardown() {
        base.Teardown();
    }

    private void EnemyCloseMove() {
        float distance = Vector3.Distance(GetPlayerPosition(), GetEnemyPosition());

        Vector3 norm = (GetPlayerPosition() - GetEnemyPosition()).normalized;

        _enemyRigidbody.velocity = norm * 10.0f;

        if(distance < _PLAYER_DISTANCE) {
            GetEnemy()._myAI.ChangeState(new EnemyAI001_Wait());
        }
    }
}
