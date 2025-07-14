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

public class EnemyCharacter : CharacterBase {
    // 敵のHPゲージ
    protected Slider _enemyHPGauge = null;
    public CharacterAIMachine<EnemyCharacter> _myAI { get; protected set; } = null;

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
}
