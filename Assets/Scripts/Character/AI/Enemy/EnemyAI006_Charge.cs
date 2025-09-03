using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyCommonModule;
using static CharacterUtility;
using Unity.VisualScripting;

public class EnemyAI006_Charge : CharacterAIBase<EnemyCharacter> {
    private Rigidbody _enemyRigidbody = null;

    private Vector3 startPos = Vector3.zero;
    private Vector3 targetPos = Vector3.zero;
    private float chargeTime = -1;

    private const float _CHARGE_TIME = 3.0f;
    private const float _MOVE_SPEED = 15.0f;

    public override void Initialize() {
        base.Initialize();
    }
    public override void Setup() {
        base.Setup();
        _enemyRigidbody = ownerClass.GetComponent<Rigidbody>();
        chargeTime = 0.0f;
        startPos = ownerClass.transform.position;
        targetPos = GetPlayerPosition();
        LookAtPlayer();
        GetEnemy().GetEnemyAnimator().SetBool("isCharge", true);
        EnemySideRotation();

    }
    public override void Execute() {
        base.Execute();
        chargeTime += Time.deltaTime;
        Vector3 norm = (targetPos - startPos).normalized;

        norm.y = 0.0f;

        _enemyRigidbody.velocity = norm * _MOVE_SPEED;

        if (chargeTime > _CHARGE_TIME) {
            _enemyRigidbody.velocity = Vector3.zero;
            GetEnemy().GetEnemyAnimator().SetBool("isCharge", false);
            GetEnemy().SetRotation(Quaternion.identity);
            GetEnemy()._myAI.ChangeState(new EnemyAI001_Wait());
        }
    }
    public override void Teardown() {
        base.Teardown();
    }

}
