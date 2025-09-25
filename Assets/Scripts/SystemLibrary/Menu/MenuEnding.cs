using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuEnding : MenuBase {
    [SerializeField]
    private Image _endingImage = null;
    [SerializeField]
    private Image _bgImage = null;
    [SerializeField]
    private Sprite[] _endingSprite = null;
    private bool _isGameClear = false;

    private CancellationToken _token;

    public override async UniTask Initialize() {
        await base.Initialize();
        _isGameClear = false;
    }

    public override async UniTask Open() {
        await base.Open();
        int resultIndex;
        if (_isGameClear) {
            resultIndex = 1;
            _bgImage.color = Color.white;
        } else {
            resultIndex = 0;
            _bgImage.color = Color.black;
        }
        _endingImage.sprite = _endingSprite[resultIndex];
        await FadeManager.Instance.FadeIn(FadeType.White);
        SoundManager.Instance.PlayBGM(0);
        _token = this.GetCancellationTokenOnDestroy();
        while (true) {
            if(Input.anyKey) break;

            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _token);
        }
        SoundManager.Instance.StopBGM();
        await FadeManager.Instance.FadeOut(FadeType.White);
        await Close();
    }

    public override async UniTask Close() {
        await base.Close();
        _isGameClear = false;
    }

    public void SetGameClear(bool setFlag) {
        _isGameClear = setFlag;
    }
}
