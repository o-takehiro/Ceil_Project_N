/*
 * @file    CharacterAIMachine.cs
 * @brief   キャラクターAIの管理
 * @author  Seki
 * @date    2025/7/9
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAIMachine<T> where T : class {
    //状態の持ち主
    public T ownerClass = null;
    //現在のステート
    public static CharacterAIBase<T> currentState { get; private set; } = null;

    public void Setup(T setOwnerClass) {
        ownerClass = setOwnerClass;
    }

    public void Update() {
        currentState?.Execute();
    }
    /// <summary>
    /// ステートの変更
    /// </summary>
    /// <param name="setState"></param>
    public void ChangeState(CharacterAIBase<T> setState) {
        currentState?.Teardown();
        currentState = setState;
        currentState.SetOwner(ownerClass);
        currentState.Setup();
    }
}
