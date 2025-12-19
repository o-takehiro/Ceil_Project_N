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
    // チュートリアル用画像
    [SerializeField]
    private Image _guideImage = null;
    // 元となる画像リスト
    [SerializeField]
    private Sprite[] _originSpriteList = null;
    // ページ数テキスト
    [SerializeField]
    private TextMeshProUGUI _pageNumText = null;
    // 入力受付
    private CameraInputActions _myInput;
    // 画像リスト
    private Sprite[] _guideSpriteList = null;
    // メニュー開閉フラグ
    private bool _isClose = false;
    // ページ数
    private int _pageNum = -1;
    // 最大ページ
    private const int _MAX_PAGE_NUM = 4;

    private CancellationToken _token;

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
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
    /// <summary>
    /// 開く
    /// </summary>
    /// <returns></returns>
    public override async UniTask Open() {
        await base.Open();
        _token = this.GetCancellationTokenOnDestroy();
        _pageNum = 0;
        // 入力の登録
        _myInput.UI.NextPage.performed += NextPage;
        _myInput.UI.PrevPage.performed += PrevPage;
        // 入力の有効化
        _myInput.UI.Enable();
        _pageNumText.text = (_pageNum + 1) + " / " + _MAX_PAGE_NUM.ToString();

        while (!_isClose) {
            await UniTask.DelayFrame(1, PlayerLoopTiming.Update, _token);
        }
        await Close();
    }
    /// <summary>
    /// 閉じる
    /// </summary>
    /// <returns></returns>
    public override async UniTask Close() {
        await base.Close();
        _isClose = false;
        _pageNum = 0;
        _myInput.UI.Submit.performed -= NextPage;
        _myInput.UI.Submit.performed -= PrevPage;
        _myInput.UI.Disable();
    }
    /// <summary>
    /// 次のページに進む(InputAction用)
    /// </summary>
    /// <param name="context"></param>
    public void NextPage(InputAction.CallbackContext context) {
        ExecuteNextPage();
    }
    /// <summary>
    /// 前のページに戻る(InputAction用)
    /// </summary>
    /// <param name="context"></param>
    public void PrevPage(InputAction.CallbackContext context) {
        ExecutePrevPage();
    }
    /// <summary>
    /// 次のページに進む
    /// </summary>
    public void ExecuteNextPage() {
        SoundManager.Instance.PlaySE(18);
        if(_pageNum + 1 == _MAX_PAGE_NUM) {
            CloseMenu();
            return;
        }
        _pageNum++;
        _guideImage.sprite = _guideSpriteList[_pageNum];
        _pageNumText.text = (_pageNum + 1) + " / " + _MAX_PAGE_NUM.ToString();
    }
    /// <summary>
    /// 前のページに戻る
    /// </summary>
    public void ExecutePrevPage() {
        SoundManager.Instance.PlaySE(18);
        if (_pageNum <= 0) return;
        _pageNum--;
        _guideImage.sprite = _guideSpriteList[_pageNum];
        _pageNumText.text = (_pageNum + 1) + " / " + _MAX_PAGE_NUM.ToString();
    }
    /// <summary>
    /// メニューを閉じる
    /// </summary>
    public void CloseMenu() {
        _isClose = true;
    }
}
