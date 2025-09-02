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
    protected GameObject _enemyCanvas = null;
    protected Slider _enemyHPGauge = null;
    public CharacterAIMachine<EnemyCharacter> _myAI { get; protected set; } = null;
    public CharacterAIBase<EnemyCharacter> _actionMachine { get; protected set; } = null;
    public List<eMagicType> _currentMagic { get; protected set; } = null;

    public override void Initialize() {
        base.Initialize();
    }
    public override void Setup() {
        base.Setup();
    }
    public override void Teardown() {
        base.Teardown();
    }
    /// <summary>
    /// プレイヤーの判別
    /// </summary>
    /// <returns></returns>
    public override bool isPlayer() {
        return false;
    }
    
    public override void Dead() {
        UnuseEnemy();
    }

    protected void SetEnemyCanvas() {
        if(_enemyHPGauge != null) return;

        _enemyHPGauge = MenuManager.Instance.Get<EnemyHPGauge>().GetSlider();
    }

    protected void SetupCanvasPosition(float setPosY, Vector3 setSize) {
        Vector3 canvasPos = Vector3.zero;
        canvasPos.y = setPosY;
        SetEnemyCanvas();
        _enemyHPGauge.transform.SetParent(_enemyCanvas.transform);
        _enemyCanvas.transform.position = canvasPos;
        _enemyCanvas.transform.localScale = setSize;
        _enemyCanvas.gameObject.SetActive(true);
    }

    public CharacterAIBase<EnemyCharacter> GetActionMachine() {
        return _actionMachine;
    }
}
