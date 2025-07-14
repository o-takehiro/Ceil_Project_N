/*
 * @file    CharacterUtility.cs
 * @brief   キャラクター関連実行処理
 * @author  Seki
 * @date    2025/7/8
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUtility {
    /// <summary>
    /// プレイヤーキャラクター生成
    /// </summary>
    public static void UsePlayer() {
        CharacterManager.instance.UsePlayer();
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
        return CharacterManager.instance.GetPlayerPosition();
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
        return CharacterManager.instance.GetEnemyPosition();
    }
}
