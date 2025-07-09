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
using static GameConst;

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
    private List<EnemyCharacter> _originEnemyList = null;

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
        int enemyTypeMax = (int)eEnemyType.Max;
        //プレイヤー情報を未使用リストに入れる
        _unusePlayerObject = Instantiate(_originPlayerObject, _unuseObjectRoot);
        _unuseEnemyList = new List<EnemyCharacter>();
        //敵情報を未使用リストに入れる
        for (int i = 0; i < enemyTypeMax; i++) {
            _unuseEnemyList.Add(Instantiate(_originEnemyList[i], _unuseObjectRoot));
        }
    }
    /// <summary>
    /// プレイヤーキャラクター生成
    /// </summary>
    public void UsePlayer() {
        //プレイヤー情報のインスタンスを未使用オブジェクトから取得
        PlayerCharacter player = _unusePlayerObject;
        //未使用プレイヤーオブジェクトを空にする
        _unusePlayerObject = null;
        //親オブジェクトの移動
        player.transform.SetParent(_useObjectRoot);
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
        _unuseEnemyList[ID] = null;
        //親オブジェクトの移動
        enemy.transform.SetParent(_useObjectRoot);
        //敵の使用準備
        enemy.Setup();
    }
    /// <summary>
    /// プレイヤーを未使用状態にする
    /// </summary>
    /// <param name="unusePlayer"></param>
    public void UnusePlayer(PlayerCharacter unusePlayer) {
        if(unusePlayer == null) return;

        //未使用プレイヤーオブジェクトに入れる
        _unusePlayerObject = unusePlayer;
        _unusePlayerObject.Teardown();
        _unusePlayerObject.transform.SetParent(_unuseObjectRoot);
    }
    /// <summary>
    /// 敵を未使用状態にする
    /// </summary>
    /// <param name="ID"></param>
    public void UnuseEnemy(EnemyCharacter unuseEnemy) {
        if(unuseEnemy == null) return;

        for (int i = 0, max = (int)eEnemyType.Max; i < max; i++) {
            if (_unuseEnemyList[i] != null) continue;

            _unuseEnemyList[i] = unuseEnemy;
            _unuseEnemyList[i].Teardown();
            _unuseEnemyList[i].transform.SetParent(_unuseObjectRoot);
        }
    }
}
