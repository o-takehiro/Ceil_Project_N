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
    private GameObject[] _magicSpawnPos = new GameObject[4];  　  // 魔法を発射する場所
    public bool _isDeath = false;
    private static eMagicType _pendingMagic = eMagicType.Invalid; // 入れ替え用魔法

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="animator"></param>
    public PlayerMagicAttack() {
        InitializeLists();
        _isDeath = false;
    }

    private void InitializeLists() {
        _eMagicList = new List<eMagicType>(4);
        for (int i = 0; i < 4; i++) {
            _eMagicList.Add(eMagicType.Invalid);
        }
        _eMagicStorageList = new List<eMagicType>();
    }

    /// <summary>
    /// スロットに発射位置を設定
    /// </summary>
    /// <param name="slotIndex"></param>
    /// <param name="position"></param>
    public void SetMagicSpawnPosition(int slotIndex, GameObject position) {
        if (slotIndex < 0 || slotIndex >= _magicSpawnPos.Length) return;
        _magicSpawnPos[slotIndex] = position;
        _magicSpawnPos[slotIndex].SetActive(false);
    }

    /// <summary>
    /// 魔法発射
    /// </summary>
    public void RequestAttack(int slotIndex) {
        // ★ 入れ替え待ち状態なら → 通常攻撃ではなく入れ替えにする
        if (_pendingMagic != eMagicType.Invalid) {
            ReplacePendingMagic(slotIndex);
            return;
        }

        float currentMP = CharacterUtility.GetPlayerCurrentMP();
        if (slotIndex < 0 || slotIndex >= _eMagicList.Count) return;
        var magicType = _eMagicList[slotIndex];
        if (magicType == eMagicType.Invalid) return;
        if (currentMP <= 0.0f) {
            RequestCancelMagic(slotIndex);
            return;
        }

        if (!_isDeath) {
            GameObject spawnPoint = _magicSpawnPos[slotIndex];
            if (spawnPoint == null) return;
            // 魔法発射
            CreateMagic(eSideType.PlayerSide, magicType, spawnPoint);
            // エフェクト再生
            UniTask task = EffectManager.Instance.PlayEffect(eEffectType.Book, spawnPoint.transform.position);
            // 本出現
            spawnPoint.SetActive(true);
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
        GameObject spawnPoint = _magicSpawnPos[slotIndex];
        // 渡された魔法がInvalid出なければ
        if (magicType == eMagicType.Invalid) return;
        if (spawnPoint != null) {
            spawnPoint.SetActive(false);
        }
        // 魔法発射解除
        MagicReset(eSideType.PlayerSide, magicType);
        spawnPoint.SetActive(false);
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
        var player = CharacterUtility.GetPlayer();
        if (player == null) return;
        if (_isDeath) {
            return; // 死んでいたら何も処理しない
        }
        for (int i = 0; i < _eMagicList.Count; i++) {
            float currentMP = CharacterUtility.GetPlayerCurrentMP();
            if (currentMP <= 0.0f) {
                RequestCancelMagic(i);
            }
        }

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

        // 空きがあるか試す
        for (int i = 0; i < _eMagicList.Count; i++) {
            if (_eMagicList[i] == eMagicType.Invalid) {
                _eMagicList[i] = magicType;
                SetMagicUI.Instance.UpdateMagicUI();
                Debug.Log($"{magicType} をスロット {i} にセットした");
                return;
            }
        }

        // 空きがなければ入れ替え待ちにする
        _pendingMagic = magicType;

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
                // テキストに魔法の文字列をセット
                SetMagicUI.Instance.UpdateMagicUI();
                Debug.Log($"{magicType} をスロット {i} にセットした");
                return;
            }
        }
    }

    /// <summary>
    /// 指定したスロットを新しい魔法に入れ替える
    /// </summary>
    public void ReplaceMagic(int slotIndex, eMagicType newMagic) {
        if (slotIndex < 0 || slotIndex >= _eMagicList.Count) return;
        _eMagicList[slotIndex] = newMagic;
        SetMagicUI.Instance.UpdateMagicUI();
        Debug.Log("入れ替えたよ");
    }

    /// <summary>
    /// 入れ替え待ち魔法を指定スロットにセットする
    /// </summary>
    private void ReplacePendingMagic(int slotIndex) {
        if (_pendingMagic == eMagicType.Invalid) return;
        ReplaceMagic(slotIndex, _pendingMagic);
        _pendingMagic = eMagicType.Invalid; // 待ち解除
    }

    /// <summary>
    /// 魔法リストUIの表示
    /// </summary>
    public void OpenMagicUI() {
        // 魔法リストの表示
        SetMagicUI.Instance.OpenUI();
    }

    /// <summary>
    /// 魔法リストUIの非表示
    /// </summary>
    public void CloseMagicUI() {
        // 魔法リストの非表示
        SetMagicUI.Instance.CloseUI();
    }

    // 片付け処理
    public void ResetState() {
        _isDeath = false;
    }

    /// <summary>
    /// 魔法の片付け
    /// </summary>
    public void ResetMagic() {
        InitializeLists();
    }

}
