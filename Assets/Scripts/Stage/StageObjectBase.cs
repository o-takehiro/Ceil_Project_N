/*
 *  @fili   StageObjectBase
 *  @author     oorui
 */
using UnityEngine;

public abstract class StageObjectBase : MonoBehaviour {
    /// <summary>
    /// 初期化処理
    /// </summary>
    public virtual void Initialize() { }

    /// <summary>
    /// 準備
    /// </summary>
    public virtual void SetUp() { }

    /// <summary>
    /// 更新処理
    /// </summary>
    protected virtual void Update() {
        OnUpdate();
    }

    /// <summary>
    /// 片付け処理
    /// </summary>
    public virtual void Teardown() { }

    /// <summary>
    /// 派生先での更新処理
    /// </summary>
    protected abstract void OnUpdate();
}
