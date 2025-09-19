using Cysharp.Threading.Tasks;
using UnityEngine;

public abstract class StageBase : MonoBehaviour {

    [SerializeField]
    protected StageObjectBase[] _stageObject = null;
    [SerializeField]
    private eStageState _stageState;
    public eStageState StageState => _stageState;
    public bool isStageClear { get; private set; } = false;

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Initialize() {
        gameObject.SetActive(false);
        await UniTask.CompletedTask;
    }
    
    /// <summary>
    /// 準備
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask SetUp() {
        gameObject.SetActive(true);
        SetIsStageClear(false);
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 実行処理
    /// </summary>
    /// <returns></returns>
    public abstract UniTask Execute();

    /// <summary>
    /// 片付け処理
    /// </summary>
    /// <returns></returns>
    public virtual async UniTask Teardown() {
        gameObject.SetActive(false);
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// ステージクリアフラグの変更
    /// </summary>
    /// <param name="setFlag"></param>
    public void SetIsStageClear(bool setFlag) {
        isStageClear = setFlag;
    }

}
