using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static CharacterUtility;
using static EnemyCommonModule;

public class EnemyAI002_CloseMove : CharacterAIBase<EnemyCharacter> {
    private Rigidbody _enemyRigidbody = null;
    private const float _PLAYER_DISTANCE = 10.0f;
    private const float _MOVE_SPEED = 10.0f;
    public override void Setup() {
        base.Setup();
        _enemyRigidbody = ownerClass.GetComponent<Rigidbody>();
    }

    public override void Execute() {
        base.Execute();
        //é©êgÇ∆ìGÇ∆ÇÃãóó£
        float distance = PlayerToEnemyDistance();
        LookAtPlayer(0.01f);
        Vector3 norm = (GetPlayerPosition() - GetEnemyPosition()).normalized;
        //yç¿ïWÇÃà⁄ìÆÇêßå¿
        norm.y = 0.0f;

        _enemyRigidbody.velocity = norm * _MOVE_SPEED;

        if (distance < _PLAYER_DISTANCE) {
            _enemyRigidbody.velocity = Vector3.zero;
            GetEnemy()._myAI.ChangeState(new EnemyAI001_Wait());
        }
    }

    public override void Teardown() {
        base.Teardown();
    }
}
