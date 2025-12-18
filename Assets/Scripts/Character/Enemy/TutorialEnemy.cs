/*
 * @file    TutorialEnemy.cs
 * @brief   チュートリアル用の敵
 * @author  Seki
 * @date    2025/7/9
 */
using UnityEngine;

using static CharacterUtility;

public class TutorialEnemy : EnemyCharacter {
    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void Initialize() {
        base.Initialize();
        decision = new TutorialBossDecision();
    }
    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="masterID"></param>
    public override void Setup(int masterID) {
        base.Setup(masterID);
        // フリーズ状態にしておく
        ChangeAction(eEnemyActionType.Freeze);
        //HPゲージの更新
        SetupCanvasPosition(Vector3.one);
    }
    /// <summary>
    /// 更新処理
    /// </summary>
    private void Update() {
        //現在の位置更新
        SetPosition(transform.position);
        //現在の回転取得
        SetRotation(transform.rotation);
        //中心座標更新
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
        // 敵行動判断リスト
        ExecuteFactors();
        // 行動更新
        ExecuteAction();
        // 行動判断更新処理
        ExecuteDecision();
        //オブジェクトの座標更新
        transform.position = currentPos;
        //オブジェクトの回転更新
        transform.rotation = currentRot;
        //1フレーム前の座標更新
        SetEnemyPrevPosition();
    }
    /// <summary>
    /// ダメージ処理
    /// </summary>
    /// <param name="damage"></param>
    public override void Damage(int damage) {
        base.Damage(damage);
        if (IsDead || !enemyAnimator) return;
        AnimatorStateInfo state = enemyAnimator.GetCurrentAnimatorStateInfo(0);

        bool isPlaying = state.normalizedTime < 1.0f;
        if(isPlaying) return;
        enemyAnimator.SetTrigger("isDamage");
    }
}
