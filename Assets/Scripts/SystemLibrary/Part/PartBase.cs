/*
 *  @file   PartBase
 *  @author oorui
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲームパートの基底
/// </summary>
public abstract class PartBase : MonoBehaviour {

    /// <summary>
    /// 初期化処理
    /// ゲーム開始時に一度だけ呼ばれる
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Initialize() {
        gameObject.SetActive(false);
        await UniTask.CompletedTask;
    }


    /// <summary>
    /// パート実行前に準備
    /// パートに遷移する前に呼ばれる
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask SetUp() {
        gameObject.SetActive(true);
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// パートの実行
    /// </summary>
    /// <returns></returns>
    public abstract UniTask Execute();

    /// <summary>
    /// パート終了時の片付け
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Teardown() {
        gameObject.SetActive(false);
        await UniTask.CompletedTask;
    }

}
