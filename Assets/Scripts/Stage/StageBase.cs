using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class StageBase : MonoBehaviour {

    [SerializeField]
    protected StageObjectBase[] _stageObject = null;


    /// <summary>
    /// ‰Šú‰»ˆ—
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Initialize() {
        gameObject.SetActive(false);
        await UniTask.CompletedTask;
    }
    
    /// <summary>
    /// €”õ
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask SetUp() {
        gameObject.SetActive(true);
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// Àsˆ—
    /// </summary>
    /// <returns></returns>
    public abstract UniTask Execute();

    /// <summary>
    /// •Ğ•t‚¯ˆ—
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Teardown() {
        gameObject.SetActive(false);
        await UniTask.CompletedTask;
    }


}
