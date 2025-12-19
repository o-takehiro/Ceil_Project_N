using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class DeathAninationAction : StateMachineBehaviour {
    /// <summary>
    /// 死亡アニメーション開始時に物理判定を消す
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.applyRootMotion = true;
        // Collider を無効化
        Collider col = animator.GetComponent<CapsuleCollider>();
         if (col != null) col.enabled = false;
    }
    /// <summary>
    /// 死亡アニメーション終了時に非表示にする
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="stateInfo"></param>
    /// <param name="layerIndex"></param>
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        GetEnemy().enemyAnimator.SetBool("isDead", false);
        StageManager.Instance.SetCurrentStageClear(true);
        animator.applyRootMotion = false;
        Collider col = animator.GetComponent<CapsuleCollider>();
        if (col != null) col.enabled = true;
        UnuseEnemy();
    }
}
