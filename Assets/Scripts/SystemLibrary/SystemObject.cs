using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ゲーム全体の機能の基底
/// </summary>
public abstract class SystemObject : MonoBehaviour {

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public abstract UniTask Initialize();

}
