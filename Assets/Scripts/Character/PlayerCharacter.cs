/*
 * @file    PlayerCharacter.cs
 * @brief   プレイヤーキャラクター情報
 * @author  Orui
 * @date    2025/7/8
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : CharacterBase {
    //プレイヤーの基礎移動スピード
    private const float _PLAYER_RAW_MOVE_SPEED = 10.0f;
    //現在のスピード
    public float playerMoveSpeed { get; private set; } = -1.0f;
    //カメラとの距離
    private float _cameraDistance = -1.0f;

    //マスターデータ依存の変数
    public int maxMP { get; private set; } = -1;
    public int MP { get; private set; } = -1;

}
