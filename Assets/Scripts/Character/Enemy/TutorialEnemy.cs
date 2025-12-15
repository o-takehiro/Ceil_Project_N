/*
 * @file    TutorialEnemy.cs
 * @brief   チュートリアル用の敵
 * @author  Seki
 * @date    2025/7/9
 */
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

using static CharacterUtility;
using static GameConst;

public class TutorialEnemy : EnemyCharacter {
    private const float _CANVAS_POS_Y = 4.0f;

    public override void Initialize() {
        base.Initialize();
        actionMachine = new EnemyAI004_TutorialEnemyAction();
        actionMachine.Initialize();
        myAI = new CharacterAIMachine<EnemyCharacter>();
        enemyAnimator = GetComponent<Animator>();
        magicTypeList = new List<eMagicType>(MAX_ENEMY_MAGIC);
    }
    public override void Setup(int masterID) {
        base.Setup(masterID);
        //HPゲージの更新
        SetupCanvasPosition(Vector3.one);
        //ステートマシーンの初期化
        myAI.Setup(this);
        myAI.ChangeState(new EnemyAI010_LookAtPlayer());
    }
    private void Update() {
        //現在の位置更新
        SetPosition(transform.position);
        //現在の回転取得
        SetRotation(transform.rotation);
        //中心座標更新
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
        //AIマシーンの更新
        myAI.Update();
        //オブジェクトの座標更新
        transform.position = currentPos;
        //オブジェクトの回転更新
        transform.rotation = currentRot;
        //1フレーム前の座標更新
        SetEnemyPrevPosition();
    }

    public override void Damage(int damage) {
        base.Damage(damage);
        if (isDead || enemyAnimator.GetCurrentAnimatorStateInfo(0).IsName("isMagicAttack"))
            return;

        enemyAnimator.SetTrigger("isDamage");
    }
}
