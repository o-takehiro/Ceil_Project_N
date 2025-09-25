using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

public class MenuTutorialGuide : MenuBase {
    [SerializeField]
    private Image _guideImage = null;
    [SerializeField]
    private Sprite[] _originSpriteList = null;
    [SerializeField]
    private TextMeshProUGUI _pageNumText = null;

    private CameraInputActions _myInput;
    private Sprite[] _guideSpriteList = null;
    private bool _isClose = false;
    private int _pageNum = -1;
    
    private const int _MAX_PAGE_NUM = 4;
    private CancellationToken _token;

    public override async UniTask Initialize() {
        await base.Initialize();
        gameObject.SetActive(false);
        _myInput = new CameraInputActions();
        _guideSpriteList = new Sprite[_MAX_PAGE_NUM];
        for (int i = 0; i < _MAX_PAGE_NUM; i++) {
            _guideSpriteList[i] = _originSpriteList[i];
        }
        _guideImage.sprite = _guideSpriteList[0];
    }

    public override async UniTask Open() {
        await base.Open();
        _token = this.GetCancellationTokenOnDestroy();
        _pageNum = 0;
        _myInput.UI.Enable();
        _myInput.UI.NextPage.performed += NextPage;
        _myInput.UI.PrevPage.performed += PrevPage;
        _pageNumText.text = (_pageNum + 1) + " / " + _MAX_PAGE_NUM.ToString();
        while (!_isClose) {
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _token);
        }
        await Close();
    }

    public override async UniTask Close() {
        await base.Close();
        _myInput.UI.Submit.performed -= NextPage;
        _myInput.UI.Submit.performed -= PrevPage;
        _myInput.UI.Disable();
        _pageNum = 0;
        _isClose = false;
    }

    public void NextPage(InputAction.CallbackContext context) {
        NextPage();
    }

    public void PrevPage(InputAction.CallbackContext context) {
        PrevPage();
    }
    public void NextPage() {
        if(_pageNum + 1 == _MAX_PAGE_NUM) {
            CloseMenu();
            return;
        }
        _pageNum++;
        _guideImage.sprite = _guideSpriteList[_pageNum];
        _pageNumText.text = (_pageNum + 1) + " / " + _MAX_PAGE_NUM.ToString();
    }

    public void PrevPage() {
        if(_pageNum <= 0) return;
        _pageNum--;
        _guideImage.sprite = _guideSpriteList[_pageNum];
        _pageNumText.text = (_pageNum + 1) + " / " + _MAX_PAGE_NUM.ToString();
    }

    public void CloseMenu() {
        _isClose = true;
    }
}
