using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
using static MagicUtility;

public class EnemyAI004_TutorialEnemyAction : CharacterAIBase<EnemyCharacter> {
    private const int _DEFENSE_DISTANCE = 15;
    private const int _CLOSE_DISTANCE = 25;

    public override void Initialize() {
        base.Initialize();

    }
    public override void Setup() {
        base.Setup();
        CancelAllEnemyMagic();
    }
    public override void Execute() {
        base.Execute();
        float distance = GetPlayerToEnemyDistance();

        //プレイヤーとの距離が一定以下の場合
        if(distance < _DEFENSE_DISTANCE) {
            //魔法生成
            //GetEnemy().GetEnemyAnimator().SetTrigger("isMagicDefense");
            AddEnemyMagicType(eMagicType.Defense);
            CreateMagic(eSideType.EnemySide, GetEnemyMagicType(eMagicType.Defense));
            GetEnemy().myAI.ChangeState(new EnemyAI003_LeaveMove());
        }
        else if (distance > _CLOSE_DISTANCE) {
            GetEnemy().myAI.ChangeState(new EnemyAI002_CloseMove());
        } else {
            AddEnemyMagicType(eMagicType.MiniBullet);
            GetEnemy().GetEnemyAnimator().SetTrigger("isMagicAttack");
            GetEnemy().myAI.ChangeState(new EnemyAI001_Wait());
        }
    }
    public override void Teardown() {
        base.Teardown();

    }
}
