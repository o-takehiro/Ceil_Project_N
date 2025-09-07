/*
 * @file    EnemyCharacter.cs
 * @brief   敵キャラクター情報
 * @author  Seki
 * @date    2025/7/8
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static CharacterUtility;

public class EnemyCharacter : CharacterBase {
    // 敵のHPゲージ
    [SerializeField]
    protected GameObject enemyCanvas = null;
    protected Slider enemyHPGauge = null;
    protected Animator enemyAnimator = null;
    public CharacterAIMachine<EnemyCharacter> _myAI { get; protected set; } = null;
    public CharacterAIBase<EnemyCharacter> _actionMachine { get; protected set; } = null;
    public List<eMagicType> _currentMagic { get; protected set; } = null;

    public override void Initialize() {
        base.Initialize();
    }
    public override void Setup(int masterID) {
        base.Setup(masterID);
    }
    public override void Teardown() {
        base.Teardown();
        enemyHPGauge.value = 1.0f;
    }
    /// <summary>
    /// プレイヤーの判別
    /// </summary>
    /// <returns></returns>
    public override bool isPlayer() {
        return false;
    }
    
    public override void Dead() {
        _myAI.ChangeState(new EnemyAI008_Empty());
        enemyAnimator.SetTrigger("isDead");
    }

    protected void SetEnemyCanvas() {
        if(enemyHPGauge != null) return;

        enemyHPGauge = MenuManager.Instance.Get<EnemyHPGauge>().GetSlider();
    }

    protected void SetupCanvasPosition(float setPosY, Vector3 setSize) {
        Vector3 canvasPos = Vector3.zero;
        canvasPos.y = setPosY;
        SetEnemyCanvas();
        enemyHPGauge.transform.SetParent(enemyCanvas.transform);
        enemyCanvas.transform.position = canvasPos;
        enemyCanvas.transform.localScale = setSize;
        enemyCanvas.gameObject.SetActive(true);
    }

    public CharacterAIBase<EnemyCharacter> GetActionMachine() {
        return _actionMachine;
    }

    public Animator GetEnemyAnimator() {
        return enemyAnimator;
    }

    public Slider GetEnemySlider() {
        return enemyHPGauge;
    }

    public float GetEnemySliderValue() {
        return HP / maxHP;
    }
}
