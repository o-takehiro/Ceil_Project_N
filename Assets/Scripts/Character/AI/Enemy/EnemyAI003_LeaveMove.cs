using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static CharacterUtility;

public class EnemyAI003_LeaveMove : CharacterAIBase<EnemyCharacter> {
    private Rigidbody _enemyRigidbody = null;
    private float randomAngle = -1;
    //定数
    private const float _PLAYER_DISTANCE = 20.0f;
    private const float _MOVE_SPEED = 10.0f;
    private const float _RANGE_ANGLE = 60.0f;
    private const float _BEHAIND_ANGLE = 180.0f;
    public override void Setup() {
        base.Setup();
        Vector3 enemyAngle = GetEnemy().transform.localEulerAngles;
        _enemyRigidbody = ownerClass.GetComponent<Rigidbody>();
        //背後から逃げる方向（角度を）をランダムで決める
        float minAngle = _BEHAIND_ANGLE - _RANGE_ANGLE;
        float maxAngle = _BEHAIND_ANGLE + _RANGE_ANGLE;
        randomAngle = Random.Range(minAngle, maxAngle);
    }

    public override void Execute() {
        base.Execute();
        //背後を向く
        //プレイヤーとの距離を取得
        float distance = PlayerToEnemyDistance();
        //逃げる方向ベクトルの宣言
        Vector3 direction;
        //ランダムなデグリー角からラジアン角に変換する
        float escapeAngle = randomAngle * Mathf.PI / _BEHAIND_ANGLE;
        //方向ベクトルへ変換する
        direction.x = Mathf.Cos(escapeAngle);
        direction.y = 0;
        direction.z = Mathf.Sin(escapeAngle);

        if (distance > _PLAYER_DISTANCE) {
            _enemyRigidbody.velocity = Vector3.zero;
            GetEnemy()._myAI.ChangeState(new EnemyAI001_Wait());
        } else {
            _enemyRigidbody.velocity = direction * _MOVE_SPEED;
        }
    }

    public override void Teardown() {
        base.Teardown();
    }
}
