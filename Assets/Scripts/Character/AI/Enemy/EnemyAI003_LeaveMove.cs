using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static CharacterUtility;
using static EnemyCommonModule;

public class EnemyAI003_LeaveMove : CharacterAIBase<EnemyCharacter> {
    private Rigidbody _enemyRigidbody = null;
    private float _randomAngle = -1;
    //定数
    private const float _PLAYER_DISTANCE = 20.0f;
    private const float _MOVE_SPEED = 10.0f;
    private const float _RANGE_ANGLE = 30.0f;
    public override void Setup() {
        base.Setup();
        Vector3 enemyAngle = GetEnemy().transform.localEulerAngles;
        _enemyRigidbody = ownerClass.GetComponent<Rigidbody>();
        //背後から逃げる方向（角度を）をランダムで決める
        _randomAngle = Random.Range(-_RANGE_ANGLE, _RANGE_ANGLE);
    }

    public override void Execute() {
        base.Execute();
        //プレイヤーとの距離を取得
        float distance = PlayerToEnemyDistance();

        //敵の真後ろのベクトルを取得
        Vector3 centerDir = -GetEnemy().transform.forward;
        //中心角度を求める(デグリー角)
        float centerAngle = Mathf.Atan2(centerDir.z, centerDir.x) * Mathf.Rad2Deg;

        //ランダム角度を生成(デグリー角)
        float angle = centerAngle + _randomAngle;

        //ラジアン角に変換してベクトルを再生成
        Vector3 moveDirection = Vector3.zero;
        float rad = angle * Mathf.Deg2Rad;
        moveDirection.x = Mathf.Cos(rad);
        moveDirection.z = Mathf.Sin(rad);

        //進む方向に向く
        //LookAtDirection(centerDir);

        if (distance > _PLAYER_DISTANCE) {
            _enemyRigidbody.velocity = Vector3.zero;
            GetEnemy()._myAI.ChangeState(new EnemyAI001_Wait());
        } else {
            _enemyRigidbody.velocity = moveDirection * _MOVE_SPEED;
        }
    }

    public override void Teardown() {
        base.Teardown();
    }
}
