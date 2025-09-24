using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class MenuTutorialGuide : MenuBase {
    [SerializeField]
    private Image _guideImage = null;
    [SerializeField]
    private Sprite[] _originSpriteList = null;

    private CameraInputActions _myInput;
    private Sprite[] _guideSpriteList = null;
    private bool _isClose = false;
    private int _pageNum = -1;
    
    private const int _MAX_PAGE_NUM = 4;
    private CancellationToken _token;

    public override async UniTask Initialize() {
        await base.Initialize();
        _myInput = new CameraInputActions();
        for (int i = 0; i < _MAX_PAGE_NUM; i++) {
            _guideSpriteList[i] = _originSpriteList[i];
        }
        _guideImage.sprite = _guideSpriteList[0];
    }

    public override async UniTask Open() {
        await base.Open();
        _token = this.GetCancellationTokenOnDestroy();
        _pageNum = 0;
        while (!_isClose) {
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _token);
        }
        await Close();
    }

    public override async UniTask Close() {
        await base.Close();
        _pageNum = 0;
        _isClose = false;
    }

    public void NextPage() {
        _pageNum++;
        _guideImage.sprite = _guideSpriteList[_pageNum];
    }

    public void PrevPage() {
        _pageNum--;
        _guideImage.sprite = _guideSpriteList[_pageNum];
    }

    public void CloseMenu() {
        _isClose = true;
    }
}
