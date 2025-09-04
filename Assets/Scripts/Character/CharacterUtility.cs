/*
 * @file    CharacterUtility.cs
 * @brief   キャラクター関連実行処理
 * @author  Seki
 * @date    2025/7/8
 */
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharacterUtility {
    /// <summary>
    /// プレイヤーキャラクター生成
    /// </summary>
    public static void UsePlayer(int masterID) {
        CharacterManager.instance.UsePlayer(masterID);
    }
    /// <summary>
    /// 敵キャラクター生成
    /// </summary>
    /// <param name="ID"></param>
    public static void UseEnemy(int ID) {
        CharacterManager.instance.UseEnemy(ID);
    }
    /// <summary>
    /// プレイヤーを未使用状態にする
    /// </summary>
    /// <param name="unusePlayer"></param>
    public static void UnusePlayer() {
        CharacterManager.instance.UnusePlayer();
    }
    /// <summary>
    /// 敵を未使用状態にする
    /// </summary>
    /// <param name="unuseEnemy"></param>
    public static void UnuseEnemy() {
        CharacterManager.instance.UnuseEnemy();
    }
    /// <summary>
    /// プレイヤー取得
    /// </summary>
    /// <returns></returns>
    public static PlayerCharacter GetPlayer() {
        return CharacterManager.instance.GetPlayer();
    }
    /// <summary>
    /// プレイヤーの座標取得
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetPlayerPosition() {
        return GetPlayer().GetCurrentPosition();
    }
    /// <summary>
    /// プレイヤーの1フレーム前の座標取得
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetPlayerPrevPos() {
        return GetPlayer().GetPrevPosition();
    }
    /// <summary>
    /// プレイヤーの現在のフレームから一フレーム前を引いた移動量計算
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetPlayerMoveDelta() {
        return GetPlayerPosition() - GetPlayerPrevPos();
    }
    /// <summary>
    /// プレイヤーの回転取得
    /// </summary>
    /// <returns></returns>
    public static Quaternion GetPlayerRotation() {
        return GetPlayer().GetRotation();
    }
    /// <summary>
    /// プレイヤーの座標設定
    /// </summary>
    /// <param name="setPosition"></param>
    public static void SetPlayerPosition(Vector3 setPosition) {
        GetPlayer().SetPosition(setPosition);
    }
    /// <summary>
    /// プレイヤーの1フレーム前の座標設定
    /// </summary>
    public static void SetPlayerPrevPosition() {
        GetPlayer().SetPrevPosition();
    }
    /// <summary>
    /// プレイヤー回転設定
    /// </summary>
    /// <param name="setRotation"></param>
    public static void SetPlayerRotation(Quaternion setRotation) {
        GetPlayer().SetRotation(setRotation);
    }
    /// <summary>
    /// 敵取得
    /// </summary>
    /// <returns></returns>
    public static EnemyCharacter GetEnemy() {
        return CharacterManager.instance.GetEnemy();
    }
    /// <summary>
    /// 敵座標取得
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetEnemyPosition() {
        return GetEnemy().GetCurrentPosition();
    }
    /// <summary>
    /// 敵の1フレーム前の座標取得
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetEnemyPrevPosition() {
        return GetEnemy().GetPrevPosition();
    }
    /// <summary>
    /// 敵の現在のフレームから一フレーム前を引いた移動量計算
    /// </summary>
    /// <returns></returns>
    public static Vector3 GetEnemyMoveDelta() {
        return GetEnemyPosition() - GetEnemyPrevPosition();
    }
    /// <summary>
    /// 敵回転取得
    /// </summary>
    /// <returns></returns>
    public static Quaternion GetEnemyRotation() {
        return GetEnemy().GetRotation();
    }
    /// <summary>
    /// 敵座標の設定
    /// </summary>
    /// <param name="setRotation"></param>
    public static void SetEnemyPosition(Vector3 setPosition) {
        GetEnemy().SetPosition(setPosition);
    }
    /// <summary>
    /// 敵の1フレーム前の座標設定
    /// </summary>
    public static void SetEnemyPrevPosition() {
        GetEnemy().SetPrevPosition();
    }
    /// <summary>
    /// 敵回転設定
    /// </summary>
    /// <param name="setRotation"></param>
    public static void SetEnemyRotation(Quaternion setRotation) {
        GetEnemy().SetRotation(setRotation);
    }
    /// <summary>
    /// プレイヤーへのダメージ
    /// </summary>
    /// <param name="setValue"></param>
    public static void ToPlayerDamage(int setValue) {
        GetPlayer().RemoveHP(setValue);
    }
    /// <summary>
    /// 敵へのダメージ
    /// </summary>
    /// <param name="setValue"></param>
    public static void ToEnemyDamage(int setValue) {
        GetEnemy().RemoveHP(setValue);
    }
    /// <summary>
    /// プレイヤーと敵との距離の取得
    /// </summary>
    /// <returns></returns>
    public static float GetPlayerToEnemyDistance() {
        return Vector3.Distance(GetPlayerPosition(), GetEnemyPosition());
    }

    public static CharacterAIBase<EnemyCharacter> GetActionMachine() {
        return GetEnemy().GetActionMachine();
    }
}
