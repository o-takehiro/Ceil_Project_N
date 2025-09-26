using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static EnemyCommonModule;
using static CharacterUtility;
using static MagicUtility;

public class EnemyAI006_Charge : CharacterAIBase<EnemyCharacter> {
    private Rigidbody _enemyRigidbody = null;

    private Vector3 _startPos = Vector3.zero;
    private Vector3 _targetPos = Vector3.zero;
    private float _chargeTime = -1;
    private bool _isClosePlayer = false;

    private const float _CHARGE_TIME = 3.0f;
    private const float _MOVE_SPEED = 15.0f;
    private const float _CLOSE_DISTANCE = 15.0f;
    private const float _LEAVE_DISTANCE = 20.0f;

    public override void Initialize() {
        base.Initialize();
    }
    public override void Setup() {
        base.Setup();
        _enemyRigidbody = ownerClass.GetComponent<Rigidbody>();
        _chargeTime = 0.0f;
        _isClosePlayer = false;
        _startPos = ownerClass.transform.position;
        _targetPos = GetPlayerPosition();
        LookAtPlayer();
        GetEnemy().GetEnemyAnimator().SetBool("isCharge", true);
        EnemySideRotation();
        GetEnemy().SetEnemyAttackValue(1);
        SetEnemyAttackCollider(GetEnemy().GetEnemyAttackValue(), true);
    }
    public override void Execute() {
        base.Execute();
        float distance = GetPlayerToEnemyDistance();
        if(distance < _CLOSE_DISTANCE) _isClosePlayer = true;
        _chargeTime += Time.deltaTime;
        Vector3 norm = (_targetPos - _startPos).normalized;

        norm.y = 0.0f;

        _enemyRigidbody.velocity = norm * _MOVE_SPEED;

        if (_chargeTime > _CHARGE_TIME || _isClosePlayer && distance > _LEAVE_DISTANCE) {
            _enemyRigidbody.velocity = Vector3.zero;
            GetEnemy().GetEnemyAnimator().SetBool("isCharge", false);
            GetEnemy().SetRotation(Quaternion.identity);
            GetEnemy().myAI.ChangeState(new EnemyAI001_Wait());
            SetEnemyAttackCollider(1, false);
        }
    }
    public override void Teardown() {
        base.Teardown();
        CancelEnemyMagic(GetEnemyMagicType(eMagicType.SatelliteOrbital));
    }
    public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") {
            ToPlayerDamage(GetEnemy().GetRawAttack());
        }
    }
}
