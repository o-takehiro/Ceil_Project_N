using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

// MagicUtility省略用
using static MagicUtility;
/// <summary>
/// プレイヤーの魔法を撃つ処理
/// </summary>
public class PlayerMagicAttack {
    private static List<eMagicType> _eMagicList;                  // 魔法を保存するリスト
    private static List<eMagicType> _eMagicStorageList;           // 取得したすべての魔法を保存するリスト
    public bool _isDeath = false;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="animator"></param>
    public PlayerMagicAttack() {
        _eMagicList = new List<eMagicType>(4);  // 保存数4
        _eMagicStorageList = new List<eMagicType>((int)eMagicType.Max); // 最大保存
        _isDeath = false;
        // 初期は使用不可
        for (int i = 0; i < 4; i++) {
            _eMagicList.Add(eMagicType.Invalid);
        }
    }

    /// <summary>
    /// 魔法発射
    /// </summary>
    public void RequestAttack(int slotIndex) {
        float currentMP = CharacterUtility.GetPlayerCurrentMP();
        if (slotIndex < 0 || slotIndex >= _eMagicList.Count) return;
        var magicType = _eMagicList[slotIndex];
        if (magicType == eMagicType.Invalid) {
            return;
        }
        if (!_isDeath && currentMP > 0.0f) {

            // 魔法発射
            CreateMagic(eSideType.PlayerSide, magicType);
            // Debug
            CharacterUtility.ToPlayerMPDamage(10);
        }
    }

    /// <summary>
    /// 魔法発射解除
    /// </summary>
    /// <param name="slotIndex"></param>
    public void RequestCancelMagic(int slotIndex) {
        if (slotIndex < 0 || slotIndex >= _eMagicList.Count) return;
        // スロット番目のeMagicTypeを渡す
        var magicType = _eMagicList[slotIndex];
        // 渡された魔法がInvalid出なければ
        if (magicType == eMagicType.Invalid) return;

        // 魔法発射解除
        MagicReset(eSideType.PlayerSide, magicType);

        if (_isDeath) {
            MagicReset(eSideType.PlayerSide, magicType);
        }
    }

    /// <summary>
    /// 解析魔法発動
    /// </summary>
    public void RequestAnalysis() {
        // SetMagicStorageSlotに魔法を保存していく
        AnalysisMagicActivate();
    }

    /// <summary>
    /// 1フレーム分の発射処理
    /// 非同期
    /// </summary>
    /// <returns></returns>
    public async UniTask MagicUpdate() {



        await UniTask.CompletedTask;
    }


    /// <summary>
    /// 魔法をリストに保存
    /// </summary>
    public void SetMagicToSlot(eMagicType magicType) {
        // 空いているリストに保存
        for (int i = 0; i < _eMagicList.Count; i++) {
            if (_eMagicList[i] == eMagicType.Invalid) {
                _eMagicList[i] = magicType;
                return;
            }
        }
    }

    /// <summary>
    /// 取得した魔法を最大数まで保存
    /// </summary>
    /// <param name="magicType"></param>
    public static void SetMagicStorageSlot(eMagicType magicType) {
        _eMagicStorageList.Add(magicType);

        TrySetMagicToSlotFromStorage(magicType);
    }

    /// <summary>
    /// リストの要素すべてを取得
    /// </summary>
    /// <returns></returns>
    public static List<eMagicType> GetMagicStorageSlot() {
        return _eMagicStorageList;

    }

    /// <summary>
    /// ストレージの中から、MagicListに保存する
    /// </summary>
    /// <param name="magicType"></param>
    private static void TrySetMagicToSlotFromStorage(eMagicType magicType) {
        for (int i = 0; i < _eMagicList.Count; i++) {
            if (_eMagicList[i] == eMagicType.Invalid) {
                _eMagicList[i] = magicType;
                Debug.Log($"{magicType} をスロット {i} にセットした");
                return;
            }
        }
    }



    // 片付け処理
    public void ResetState() {
        _isDeath = false;
    }

}
