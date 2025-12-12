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
    private bool[] _isCasting = new bool[4];                      // 魔法発射の押下入力
    private bool[] _effectPlaying = new bool[4];                  // スロットごとのエフェクト再生中フラグ
    public bool _isDeath = false;
    private static eMagicType _pendingMagic = eMagicType.Invalid; // 入れ替え用魔法

    private static readonly float _LOWER_LIMIT_MP = 0.0f;         // MPの下限値
    public static bool isPendingMagic = false;                           // 入れ替え待ちかどうか

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
    /// 入力側から、長押しの入力判定を受け取る
    /// </summary>
    public void SetCastingFlag(int slotIndex, bool flag) {
        if (slotIndex < 0 || slotIndex >= _isCasting.Length) return;
        _isCasting[slotIndex] = flag;
    }

    /// <summary>
    /// 魔法発射
    /// </summary>
    public void RequestAttack(int slotIndex) {
        if (slotIndex < 0 || slotIndex >= _eMagicList.Count) return;

        // 入れ替え中なら攻撃せずに入れ替えを優先
        if (_pendingMagic != eMagicType.Invalid) {
            ConfirmReplaceMagic(slotIndex);
            return;
        }

        // 現在のMPを取得
        float currentMP = CharacterUtility.GetPlayerCurrentMP();
        if (slotIndex < 0 || slotIndex >= _eMagicList.Count) return;
        var magicType = _eMagicList[slotIndex];
        if (magicType == eMagicType.Invalid) return;
        // MPが切れたら魔法の再生を停止
        if (currentMP <= _LOWER_LIMIT_MP) {
            RequestCancelMagic(slotIndex);
            return;
        }

        // 魔法発動処理
        if (!_isDeath) {
            GameObject spawnPoint = _magicSpawnPos[slotIndex];
            if (spawnPoint == null) return;
            // 魔法発動
            CreateMagic(eSideType.PlayerSide, magicType, spawnPoint);

            // 本の出現
            spawnPoint.SetActive(true);

            if (currentMP <= _LOWER_LIMIT_MP) {
                RequestCancelMagic(slotIndex);
                return;
            }
        }
    }

    /// <summary>
    /// 取得した魔法と現在の魔法を入れ替える
    /// </summary>
    public void ConfirmReplaceMagic(int slotIndex) {
        if (_pendingMagic == eMagicType.Invalid) return;
        if (slotIndex < 0 || slotIndex >= _eMagicList.Count) return;


        // 発動中ならキャンセルしてから入れ替える
        var currentMagic = _eMagicList[slotIndex];
        if (currentMagic != eMagicType.Invalid) {
            RequestCancelMagic(slotIndex);
        }

        // 入れ替える
        ReplaceMagic(slotIndex, _pendingMagic);
        _pendingMagic = eMagicType.Invalid;
        // UIを閉じる
        SetMagicUI.Instance.CloseChangeMagicUI();
        isPendingMagic = false;
        MagicReset(eSideType.PlayerSide, eMagicType.Defense);
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
        // エフェクト再生フラグリセット
        _effectPlaying[slotIndex] = false;

        if (_isDeath) {
            MagicReset(eSideType.PlayerSide, magicType);
        }
    }

    /// <summary>
    /// 魔法を発動した最初のみの処理
    /// </summary>
    /// <param name="slotIndex"></param>
    public void StartCasting(int slotIndex) {
        if (slotIndex < 0 || slotIndex >= _eMagicList.Count) return;
        // 魔法がセットされていなければ処理しない
        var magicType = _eMagicList[slotIndex];
        if (magicType == eMagicType.Invalid) return;
        // 現在のMPを取得
        float currentMP = CharacterUtility.GetPlayerCurrentMP();
        if (currentMP <= _LOWER_LIMIT_MP) return;

        if (_effectPlaying[slotIndex]) return; // すでに再生中なら何もしない

        GameObject spawnPoint = _magicSpawnPos[slotIndex];
        if (spawnPoint == null) return;

        // エフェクト再生
        UniTask task = EffectManager.Instance.PlayEffect(eEffectType.Book, spawnPoint.transform.position);

        _effectPlaying[slotIndex] = true;
    }


    /// <summary>
    /// 解析魔法発動
    /// </summary>
    public void RequestAnalysis() {
        // SetMagicStorageSlotに魔法を保存していく
        if (_isDeath) return;
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
        if (_isDeath) return;
        for (int i = 0; i < _eMagicList.Count; i++) {
            float currentMP = CharacterUtility.GetPlayerCurrentMP();
            if (currentMP <= _LOWER_LIMIT_MP) {
                RequestCancelMagic(i);
                continue;
            }

            // 長押しされていたら
            if (_isCasting[i]) {
                RequestAttack(i);
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
                return;
            }
        }

        // 空きがなければ入れ替え待ちにする
        _pendingMagic = magicType;
        // 入れ替え待ちフラグをON
        isPendingMagic = true;
        // 入れ替え待ちUI表示
        SetMagicUI.Instance.OpenChangeMagicUI();
        // 防御魔法を常に展開
        CreateMagic(eSideType.PlayerSide, eMagicType.Defense);

    }

    /// <summary>
    /// リストの要素すべてを取得
    /// </summary>
    /// <returns></returns>
    public static List<eMagicType> GetMagicStorageSlot() {
        return _eMagicStorageList;

    }

    /// <summary>
    /// 指定したスロットを新しい魔法に入れ替える
    /// </summary>
    public void ReplaceMagic(int slotIndex, eMagicType newMagic) {
        if (slotIndex < 0 || slotIndex >= _eMagicList.Count) return;
        _eMagicList[slotIndex] = newMagic;
        SetMagicUI.Instance.UpdateMagicUI();
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

    // 現在プレイヤーが使用している魔法のリストを取得
    public static List<eMagicType> GetEquippedMagicList() {
        return _eMagicList;
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
        MagicReset(eSideType.PlayerSide, eMagicType.Max);
        SetMagicUI.Instance.ResetMagicUI();
    }
}
