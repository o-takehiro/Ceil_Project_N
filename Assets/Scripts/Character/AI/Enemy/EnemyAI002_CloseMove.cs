using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static CharacterUtility;

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
        //自身と敵との距離
        float distance = PlayerToEnemyDistance();

        //プレイヤーの方向を向き続ける
        Quaternion enemyRotation = GetEnemyRotation();
        //方向を決める
        Vector3 direction = GetPlayerPosition() - GetEnemyPosition();
        //水平方向のみの回転のため、yには0を代入
        direction.y = 0;
        //回転させる
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        enemyRotation = Quaternion.Slerp(enemyRotation, lookRotation, 0.1f);
        //自身の回転に代入する
        SetEnemyRotation(enemyRotation);

        Vector3 norm = (GetPlayerPosition() - GetEnemyPosition()).normalized;
        //y座標の移動を制限
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
