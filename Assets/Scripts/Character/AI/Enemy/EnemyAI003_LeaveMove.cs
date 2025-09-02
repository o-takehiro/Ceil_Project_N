using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static CharacterUtility;
using static EnemyCommonModule;

public class EnemyAI003_LeaveMove : CharacterAIBase<EnemyCharacter> {
    private Rigidbody _enemyRigidbody = null;
    private float _randomAngle = -1;
    private float _moveTimePer = -1;
    private Vector3 _lookDir = Vector3.zero;
    private float _angle = -1;
    //定数
    private const float _PLAYER_DISTANCE = 20.0f;
    private const float _MOVE_SPEED = 10.0f;
    private const float _RANGE_ANGLE = 30.0f;
    private const float _MOVE_TIME = 2.0f;
    public override void Setup() {
        base.Setup();
        _moveTimePer = 0.0f;
        Vector3 enemyAngle = GetEnemy().transform.localEulerAngles;
        _enemyRigidbody = ownerClass.GetComponent<Rigidbody>();
        //背後から逃げる方向（角度を）をランダムで決める
        _randomAngle = Random.Range(-_RANGE_ANGLE, _RANGE_ANGLE);
        //敵の真後ろのベクトルを取得
        Vector3 centerDir = -GetEnemy().transform.forward;
        //中心角度を求める(デグリー角)
        float centerAngle = Mathf.Atan2(centerDir.z, centerDir.x) * Mathf.Rad2Deg;

        //ランダム角度を生成(デグリー角)
        _angle = centerAngle + _randomAngle;
    }

    public override void Execute() {
        base.Execute();
        //距離の計算
        float distance = GetPlayerToEnemyDistance();
        //移動時間の更新
        _moveTimePer += Time.deltaTime;
        if (distance > _PLAYER_DISTANCE || _moveTimePer > _MOVE_TIME) {
            _enemyRigidbody.velocity = Vector3.zero;
            GetEnemy()._myAI.ChangeState(new EnemyAI001_Wait());
        } else {
            _enemyRigidbody.velocity = LeaveMoveAngle(_angle) * _MOVE_SPEED;
        }
        //進む方向に向く
        Vector3 moveDir = GetEnemyMoveDelta();
        LookAtDirection(moveDir);
    }

    public override void Teardown() {
        base.Teardown();
        _moveTimePer = 0.0f;
    }
    /// <summary>
    /// 逃げる角度の計算
    /// </summary>
    /// <param name="setAngle"></param>
    /// <returns></returns>
    public Vector3 LeaveMoveAngle(float setAngle) {
        //ラジアン角に変換してベクトルを再生成
        Vector3 moveDirection = Vector3.zero;
        float rad = setAngle * Mathf.Deg2Rad;
        moveDirection.x = Mathf.Cos(rad);
        moveDirection.z = Mathf.Sin(rad);

        return moveDirection;
    }
}
