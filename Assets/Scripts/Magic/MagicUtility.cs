/*
 * @file    MagicUtility.cs
 * @brief   魔法関連実行処理
 * @author  Riku
 * @date    2025/7/9
 */

using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicUtility {

    /// <summary>
    /// ID指定の魔法オブジェクト取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static MagicObject GetMagicObject(int ID) {
        return MagicManager.instance.GetMagicObject(ID);
    }

    /// <summary>
    /// ID指定の魔法データ取得
    /// </summary>
    /// <param name="ID"></param>
    /// <returns></returns>
    public static MagicBase GetMagicData(int ID) {
        return MagicManager.instance.GetMagicData(ID);
    }

    /// <summary>
    /// 魔法オブジェクトを使用状態にする
    /// </summary>
    /// <param name="useID"></param>
    /// <returns></returns>
    public static MagicObject UseMagicObject(int useID, eMagicType magic) {
        return MagicManager.instance.UseMagicObject(useID, magic);
    }

    /// <summary>
    /// 魔法生成
    /// </summary>
    /// <param name="side"></param>
    /// <param name="magicID"></param>
    public static void CreateMagic(eSideType side, eMagicType magicID, GameObject setPosition = null) {
        UniTask task = MagicManager.instance.CreateMagic(side, magicID, setPosition);
    }

    /// <summary>
    /// 魔法のリセット
    /// </summary>
    public static void MagicReset(eSideType side, eMagicType magicID) {
        UniTask task = MagicManager.instance.MagicReset(side, magicID);
    }

    /// <summary>
    /// 解析魔法の発動
    /// </summary>
    public static void AnalysisMagicActivate() {
        MagicManager.instance.AnalysisMagicActivate();
    }

    /// <summary>
    /// 魔法削除
    /// </summary>
    /// <param name="removeMagic"></param>
    public static async UniTask RemoveMagic(MagicBase removeMagic) {
        await MagicManager.instance.UnuseMagicData(removeMagic);
    }

    /// <summary>
    /// 魔法オブジェクトを不可視化
    /// </summary>
    /// <param name="removeObject"></param>
    public static void RemoveMagicObject(MagicObject removeObject) {
        UniTask task = MagicManager.instance.UnuseMagicObject(removeObject);
    }

    /// <summary>
    /// 特定の魔法が発動中かどうか
    /// </summary>
    /// <param name="side"></param>
    /// <param name="magic"></param>
    /// <returns></returns>
    public static bool GetMagicActive(int side, int magic) {
        return MagicManager.instance.GetMagicActive(side, magic);
    }

    /// <summary>
    /// 全ての魔法に指定処理実行
    /// </summary>
    /// <param name="action"></param>
    public static void ExecuteAllMagic(System.Action<MagicBase> action) {
        MagicManager.instance.ExecuteAllMagic(action);
    }

    /// <summary>
    /// 魔法生成中かどうか
    /// </summary>
    /// <returns></returns>
    public static bool GetMagicGenerating() {
        return MagicManager.instance.magicGenerate;
    }
}
