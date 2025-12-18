using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1_ChargeAttack : IEnemyAction {
    // 終了判定
    private bool _isFinished = false;
    // Rigidbody
    private Rigidbody _rigidbody = null;
    private ChargeAttackCollider _chargeCollider = null;
    // 目標位置
    private Vector3 _targetPos = Vector3.zero;
    // アニメーション名
    private const string _ANIMATION_NAME = "isCharge";
    private const float _CHARGE_SPEED = 5.0f;
    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy) {
        if(!enemy) return;
        _isFinished = false;
        _rigidbody = enemy.GetComponent<Rigidbody>();
        _chargeCollider = enemy.GetComponentInChildren<ChargeAttackCollider>(true);
        _targetPos = CharacterUtility.GetPlayerPosition();
        // 角度処理
        EnemyCommonModule.EnemySideRotation(enemy);
        enemy.GetEnemyAnimator().SetBool(_ANIMATION_NAME, true);
        MagicUtility.CreateMagic(eSideType.EnemySide, eMagicType.SatelliteOrbital);

        if(!_chargeCollider) return;
        _chargeCollider.gameObject.SetActive(true);
    }
    /// <summary>
    /// 実行処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy) {
        if (!enemy || !_rigidbody) return;
        Vector3 currentPos = enemy.transform.position;
        Vector3 toTarget = _targetPos - currentPos;
        toTarget.y = 0.0f;
        // 次のフレームで進む距離
        float speed = enemy.GetMoveSpeed() + _CHARGE_SPEED;
        float moveDistance = speed * Time.deltaTime;
        // 次フレームで到達する場合
        if (toTarget.magnitude <= moveDistance) {
            _isFinished = true;
            // 位置を確定させる
            enemy.transform.position = new Vector3(_targetPos.x, currentPos.y,_targetPos.z);
            return;
        }

        // 移動処理
        Vector3 dir = toTarget.normalized;
        _rigidbody.velocity = dir * speed;
    }
    /// <summary>
    /// 片付け処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Teardown(EnemyCharacter enemy) {
        if(!enemy || !_rigidbody || !_chargeCollider) return;
        _rigidbody.velocity = Vector3.zero;
        _chargeCollider.gameObject.SetActive(false);
        enemy.GetEnemyAnimator().SetBool(_ANIMATION_NAME, false);
        MagicUtility.MagicReset(eSideType.EnemySide, eMagicType.SatelliteOrbital);
    }
    /// <summary>
    /// 終了判定
    /// </summary>
    /// <returns></returns>
    public bool IsFinished() {
        return _isFinished;
    }
}
