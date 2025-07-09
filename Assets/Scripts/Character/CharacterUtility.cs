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
}
