/*
 * @file    TutorialEnemy.cs
 * @brief   チュートリアル用の敵
 * @author  Seki
 * @date    2025/7/9
 */
using UnityEngine;

using static CharacterUtility;

public class TutorialEnemy : EnemyCharacter {
    public override void Initialize() {
        base.Initialize();
        decision = new TutorialBossDecision();
    }
    public override void Setup(int masterID) {
        base.Setup(masterID);
        //HPゲージの更新
        SetupCanvasPosition(Vector3.one);
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
        if (isDead) return;

        enemyAnimator.SetTrigger("isDamage");
    }
}
