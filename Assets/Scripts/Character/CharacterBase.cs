/*
 * @file    CharacterBase.cs
 * @brief   キャラクター情報の基底クラス
 * @author  Seki
 * @date    2025/7/8
 */
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CharacterBase : MonoBehaviour {
    // 一フレーム前の座標
    public Vector3 prevPos { get; protected set; } = Vector3.zero;
    // 現在フレームの座標
    public Vector3 currentPos { get; protected set; } = Vector3.zero;
    // 現在フレームの回転
    public Quaternion currentRot { get; protected set; } = Quaternion.identity;

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
    public virtual void Setup() {

    }
    /// <summary>
    /// 片付け処理
    /// </summary>
    public virtual void Teardown() {

    }

    public abstract bool isPlayer();

    /// <summary>
    /// 座標の取得
    /// </summary>
    /// <returns></returns>
    public Vector3 GetCurrentPosition() {
        return currentPos;
    }

    public Vector3 GetPrevPosition() {
        return prevPos;
    }
    /// <summary>
    /// 座標の設定
    /// </summary>
    /// <param name="setPosition"></param>
    public void SetPosition(Vector3 setPosition) {
        currentPos = setPosition;
    }
    /// <summary>
    /// 1フレーム前の設定
    /// </summary>
    public void SetPrevPosition() {
        prevPos = currentPos;
    }
    /// <summary>
    /// 回転の取得
    /// </summary>
    /// <returns></returns>
    public Quaternion GetRotation() {
        return currentRot;
    }
    /// <summary>
    /// 回転の設定
    /// </summary>
    /// <param name="setRotation"></param>
    public void SetRotation(Quaternion setRotation) {
        currentRot = setRotation;
    }
    /// <summary>
    /// 素の攻撃力取得
    /// </summary>
    /// <returns></returns>
    public int GetRawAttack() {
        return rawAttack;
    }
    /// <summary>
    /// 素の攻撃力設定
    /// </summary>
    /// <param name="setValue"></param>
    public virtual void SetRawAttack(int setValue) {
        rawAttack = setValue;
    }
    /// <summary>
    /// 素の防御力取得
    /// </summary>
    /// <returns></returns>
    public int GetRewDefense() {
        return rawDefense;
    }
    /// <summary>
    /// 素の防御力設定
    /// </summary>
    /// <param name="setValue"></param>
    public virtual void SetRawDefense(int setValue) {
        rawDefense = setValue;
    }
    /// <summary>
    /// 最大HP設定
    /// </summary>
    /// <param name="setValue"></param>
    public virtual void SetMaxHP(int setValue) {
        maxHP = setValue;
    }
    /// <summary>
    /// HPの設定
    /// </summary>
    /// <param name="setValue"></param>
    public virtual void SetHP(int setValue) {
        HP = Mathf.Clamp(setValue, 0, maxHP);
    }
    /// <summary>
    /// HP増加
    /// </summary>
    /// <param name="addValue"></param>
    public void AddHP(int addValue) {
        SetHP(HP + addValue);
    }
    /// <summary>
    /// HP減少
    /// </summary>
    /// <param name="removeValue"></param>
    public void RemoveHP(int removeValue) {
        SetHP(HP - removeValue); 
    }
    /// <summary>
    /// キャラクターの死亡
    /// </summary>
    public abstract void Dead();
}
