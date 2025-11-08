/*
 *  @fili   Stage
 *  @author     oorui
 */
using Cysharp.Threading.Tasks;
public class Stage : StageBase {
    /// <summary>
    /// ‰Šú‰»ˆ—
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        await base.Initialize();
        for (int i = 0, max = _stageObject.Length; i < max; i++) {
            if (_stageObject[i] == null) continue;
            _stageObject[i].Initialize();
        }
    }

    /// <summary>
    /// ‰Šú‰»
    /// </summary>
    /// <returns></returns>
    public override async UniTask SetUp() {
        await base.SetUp();

        for (int i = 0, max = _stageObject.Length; i < max; i++) {
            if (_stageObject[i] == null) continue;
            _stageObject[i].SetUp();
        }

    }

    /// <summary>
    /// •Ğ•t‚¯ˆ—
    /// </summary>
    /// <returns></returns>
    public override async UniTask Teardown() {
        await base.Teardown();
        for (int i = 0, max = _stageObject.Length; i < max; i++) {
            if (_stageObject[i] == null) continue;
            _stageObject[i].Teardown();
        }
    }

    /// <summary>
    /// Àsˆ—
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public override async UniTask Execute() {
        await UniTask.CompletedTask;
    }


}
