/*
 * @file    CharacterAIBase.cs
 * @brief   キャラクターAIの基底
 * @author  Seki
 * @date    2025/7/9
 */
using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterAIBase<T> where T : class{
    //自身の持ち主のクラス
    protected T ownerClass = null;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public virtual void Initialize() { }
    /// <summary>
    /// 使用前準備
    /// </summary>
    public virtual void Setup() { }
    /// <summary>
    /// 実行処理
    /// </summary>
    public virtual void Execute() { }
    /// <summary>
    /// 片付け処理
    /// </summary>
    public virtual void Teardown() { }

    public void SetOwner(T setOwnerClass) { 
        ownerClass = setOwnerClass;
    }
}
