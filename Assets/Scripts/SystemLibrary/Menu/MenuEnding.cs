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
    [SerializeField]
    private PressButtonAlpha _pressButton = null;
    private bool _isGameClear = false;
    private int _currentBGMIndex = -1;
    private CancellationToken _token;

    public override async UniTask Initialize() {
        await base.Initialize();
        _isGameClear = false;
    }

    public override async UniTask Open() {
        await base.Open();
        int resultIndex;
        Color setColor;
        if (_isGameClear) {
            resultIndex = 1;
            _currentBGMIndex = 6;
            _bgImage.color = Color.white;
            setColor = Color.black;
            setColor.a = 0;
        } else {
            resultIndex = 0;
            _currentBGMIndex = 5;
            _bgImage.color = Color.black;
            setColor = Color.white;
            setColor.a = 0;
        }
        _endingImage.sprite = _endingSprite[resultIndex];
        _pressButton.Setup(setColor);
        await FadeManager.Instance.FadeIn(FadeType.White);
        SoundManager.Instance.PlayBGM(_currentBGMIndex);
        await _pressButton.FadeIn();
        UniTask task = _pressButton.Execute();
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
