using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class MenuEnding : MenuBase {
    [SerializeField]
    private TextMeshProUGUI _endingText = null;
    private bool _isGameClear = false;

    private CancellationToken _token;

    public override async UniTask Initialize() {
        await base.Initialize();
        _endingText.text = null;
        _isGameClear = false;
    }

    public override async UniTask Open() {
        await base.Open();
        if (_isGameClear) {
            _endingText.text = "Game Clear";
        } else {
            _endingText.text = "Game Over";
        }
        await FadeManager.Instance.FadeIn(FadeType.White);
        _token = this.GetCancellationTokenOnDestroy();
        while (true) {
            if(Input.anyKey) break;

            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _token);
        }
        await FadeManager.Instance.FadeOut(FadeType.White);
        await Close();
    }

    public override async UniTask Close() {
        await base.Close();
        _endingText.text = null;
        _isGameClear = false;
    }

    public void SetGameClear(bool setFlag) {
        _isGameClear = setFlag;
    }
}
