/*
 * @file    CharacterBase.cs
 * @brief   キャラクター情報の基底クラス
 * @author  Seki
 * @date    2025/7/8
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour {
    // 一フレーム前の座標
    public Vector3 prevPos { get; protected set; } = Vector3.zero;
    // 現在フレームの座標
    public Vector3 currentPos { get; protected set; } = Vector3.zero;
    // 現在フレームの回転
    public Vector3 currentRot { get; protected set; } = Vector3.zero;

    // マスターデータ依存の変数
    public int ID { get; protected set; } = -1;
    public int maxHP { get; protected set; } = -1;
    public int HP { get; protected set; } = -1;
    public bool isDead { get { return HP <= 0; } }
    public int rawAttack { get; protected set; } = -1;
    public int rawDefense {  get; protected set; } = -1;

    /// <summary>
    /// マスターデータ関連の使用準備処理
    /// </summary>
    protected virtual void SetupMaster() {

    }
    /// <summary>
    /// 使用準備処理
    /// </summary>
    protected virtual void Setup() {

    }
    /// <summary>
    /// 片付け処理
    /// </summary>
    protected virtual void Teardown() {

    }

    public virtual bool isPlayer() {
        return false;
    }
}
