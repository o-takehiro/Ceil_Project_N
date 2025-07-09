using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// メニューの管理
/// </summary>
public class MenuManager : SystemObject {
    /// <summary>
    /// 自身への参照
    /// </summary>
    public static MenuManager Instance { get; private set; } = null;


    private List<MenuBase> _menuList = null;

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        Instance = this;
        _menuList = new List<MenuBase>(256);
        await UniTask.CompletedTask;
    }

    /// <summary>
    /// メニューの取得
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"> プレハブのパスと名前</param>
    /// <returns></returns>
    public T Get<T>(string name = null) where T : MenuBase {
        // キャッシュしたメニューオブジェクトから探す
        for (int i = 0, max = _menuList.Count; i < max; i++) {
            T menu = _menuList[i] as T;
            if (menu == null) continue;

            return menu;
        }


        // 見つからなければ生成する
        return Load<T>(name);
    }

    /// <summary>
    /// メニューの読み込み
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name">プレハブのパスと名前</param>
    /// <returns></returns>
    private T Load<T>(string name) where T : MenuBase {
        // 読み込み
        T menu = Resources.Load<T>(name);
        if (menu == null) return null;

        // メニューの生成
        T createMenu = Instantiate(menu, transform);
        if (createMenu == null) return null;
        // 生成したメニューは非表示にしておく
        createMenu.gameObject.SetActive(false);
        _menuList.Add(createMenu);
        return createMenu;

    }




}



