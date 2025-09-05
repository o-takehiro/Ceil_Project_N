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
    private List<EnemyCharacter> _originEnemyList = null;

    // 未使用のプレイヤーオブジェクト
    private PlayerCharacter _unusePlayerObject = null;
    // 未使用の敵オブジェクトリスト
    private List<EnemyCharacter> _unuseEnemyList = null;
    // 使用中のプレイヤーオブジェクト
    private PlayerCharacter _usePlayerObject = null;
    // 使用中の敵オブジェクト
    private EnemyCharacter _useEnemyObject = null;
    
    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize() {
        instance = this;
        int maxEnemyCount = _originEnemyList.Count;
        //プレイヤー情報を未使用リストに入れる
        _unusePlayerObject = Instantiate(_originPlayerObject, _unuseObjectRoot);
        _unuseEnemyList = new List<EnemyCharacter>(maxEnemyCount);
        //敵情報を未使用リストに入れる
        for (int i = 0; i < maxEnemyCount; i++) {
            EnemyCharacter createEnemy = Instantiate(_originEnemyList[i], _unuseObjectRoot);
            createEnemy.Initialize();
            _unuseEnemyList.Add(createEnemy);   
        }
    }
    /// <summary>
    /// プレイヤーキャラクター生成
    /// </summary>
    public void UsePlayer(int masterID) {
        //プレイヤー情報のインスタンスを未使用オブジェクトから取得
        _usePlayerObject = _unusePlayerObject;
        //未使用プレイヤーオブジェクトを空にする
        _unusePlayerObject = null;
        //親オブジェクトの移動
        _usePlayerObject.transform.SetParent(_useObjectRoot);
        //プレイヤーの使用準備
        _usePlayerObject.Setup(masterID);
    }
    /// <summary>
    /// 敵キャラクター生成
    /// </summary>
    /// <param name="ID"></param>
    public void UseEnemy(eEnemyType enemyType) {
        _useEnemyObject = _unuseEnemyList[(int)enemyType];
        //未使用敵オブジェクトを空にする
        _unuseEnemyList[(int)enemyType] = null;
        //親オブジェクトの移動
        _useEnemyObject.transform.SetParent(_useObjectRoot);
        //敵の使用準備
        _useEnemyObject.Setup((int)enemyType + 1);
    }
    /// <summary>
    /// プレイヤーを未使用状態にする
    /// </summary>
    /// <param name="unusePlayer"></param>
    public void UnusePlayer() {
        if(_usePlayerObject == null) return;
        //未使用プレイヤーオブジェクトに入れる
        _unusePlayerObject = _usePlayerObject;
        //片付け処理を呼ぶ
        _usePlayerObject.Teardown();
        _unusePlayerObject.transform.SetParent(_unuseObjectRoot);
    }
    /// <summary>
    /// 敵を未使用状態にする
    /// </summary>
    /// <param name="ID"></param>
    public void UnuseEnemy() {
        for (int i = 0, max = _originEnemyList.Count; i < max; i++) {
            if (_unuseEnemyList[i] != null) continue;

            _unuseEnemyList[i] = _useEnemyObject;
            _unuseEnemyList[i].Teardown();
            _unuseEnemyList[i].transform.SetParent(_unuseObjectRoot);
        }
    }
    /// <summary>
    /// プレイヤー取得
    /// </summary>
    /// <returns></returns>
    public PlayerCharacter GetPlayer() {
        return _usePlayerObject;
    }

    /// <summary>
    /// 敵取得
    /// </summary>
    /// <returns></returns>
    public EnemyCharacter GetEnemy() {
        return _useEnemyObject;
    }
}
