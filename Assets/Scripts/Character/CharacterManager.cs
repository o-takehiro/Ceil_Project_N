/*
 * @file    CharacterManager.cs
 * @brief   キャラクター管理クラス
 * @author  Seki
 * @date    2025/7/8
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CommonModule;

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

    // 未使用のプレイヤーオブジェクト
    private PlayerCharacter _unusePlayerObject = null;
    // 未使用の敵オブジェクトリスト
    private List<EnemyCharacter> _unuseEnemyList = null;
    // 使用中のオブジェクトリスト
    private List<CharacterBase> _useObjectList = null;
    // 未使用のオブジェクトリスト
    private List<CharacterBase> _unuseObjectList = null;
    
    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize() {
        instance = this;
        _unusePlayerObject = _originPlayerObject;
        _unuseEnemyList = new List<EnemyCharacter>();

    }
    /// <summary>
    /// プレイヤーキャラクター生成
    /// </summary>
    public void UsePlayer() {
        //プレイヤー情報のインスタンスを未使用オブジェクトから取得
        PlayerCharacter player = _unusePlayerObject;
        //未使用プレイヤーオブジェクトを空にする
        _unusePlayerObject = null;
        //プレイヤーの使用準備
        player.Setup();
    }
    /// <summary>
    /// 敵キャラクター生成
    /// </summary>
    /// <param name="ID"></param>
    public void UseEnemy(int ID) {
        EnemyCharacter enemy = _unuseEnemyList[ID];
        //未使用敵オブジェクトを空にする
        _unuseEnemyList.RemoveAt(ID);
        //敵の使用準備
        enemy.Setup();
    }
}
