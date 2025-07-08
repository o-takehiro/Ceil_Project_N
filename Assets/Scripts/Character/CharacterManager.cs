/*
 * @file    CharacterManager.cs
 * @brief   キャラクター管理クラス
 * @author  Seki
 * @date    2025/7/8
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour {
    public static CharacterManager instance { get; private set; } = null;
    // 使用中キャラクターオブジェクトの親オブジェクト
    [SerializeField]
    private Transform _useObjectRoot = null;
    // 未使用キャラクターオブジェクトの親オブジェクト
    [SerializeField]
    private Transform _unuseObjectRoot = null;

    // プレイヤーオブジェクトのオリジナル
    [SerializeField]
    private PlayerCharacter _originPlayerObject = null;
    // 敵オブジェクトのオリジナル
    [SerializeField]
    private List<CharacterBase> _originEnemyList = null;

    // 使用中のプレイヤーオブジェクト
    private PlayerCharacter _usePlayerObject = null;
    // 使用中の敵オブジェクトリスト
    private List<EnemyCharacter> _useEnemyList = null;
    // 未使用のプレイヤーオブジェクト
    private PlayerCharacter _unusePlayerObject = null;
    // 未使用の敵オブジェクトリスト
    private List<EnemyCharacter> _unuseEnemyList = null;
    
    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize() {
        instance = this;

    }
}
