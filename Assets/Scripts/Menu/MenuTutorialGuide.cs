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
    private Sprite[] _originImageList = null;

    private CameraInputActions _myInput;
    private Sprite[] _guideImageList = null;
    private int _pageNum = -1;

    private const int _MAX_PAGE_NUM = 4;
    private CancellationToken _token;

    public override async UniTask Initialize() {
        await base.Initialize();
        _myInput = new CameraInputActions();
        for (int i = 0; i < _MAX_PAGE_NUM; i++) {
            _guideImageList[i] = _originImageList[i];
        }
        _guideImage.sprite = _guideImageList[0];
    }

    public override async UniTask Open() {
        await base.Open();
        _token = this.GetCancellationTokenOnDestroy();
        _pageNum = 0;
        while (true) {
            _guideImage.sprite = _guideImageList[_pageNum];

            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _token);
        }
    }

    public override async UniTask Close() {
        await base.Close();
    }
}
