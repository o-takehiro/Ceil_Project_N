/*
 *  @file    PlayerMagicAttack.cs
 *  @author  oorui
 */

using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using static MagicUtility;

/// <summary>
/// プレイヤーの魔法攻撃
/// </summary>
public class PlayerMagicAttack {

    private const int MaxSlots = 4;                        // 魔法スロット数
    private const float DefaultLowerLimitMP = 0.0f;        // MP下限
    private const int ReplaceMagicSEId = 19;               // 入れ替え時に鳴らすSE ID


    private static List<eMagicType> _equippedMagics;       // 現在装備している魔法
    private static List<eMagicType> _acquiredMagics;       // 取得したすべての魔法を保存するリスト
    private static eMagicType _pendingMagic = eMagicType.Invalid; // 入れ替え待ちの魔法
    public static bool isPendingMagic = false;           // 入れ替え待ちフラグ


    private GameObject[] _magicSpawnPos = new GameObject[MaxSlots]; //各スロットの発射位置
    private bool[] _isCasting = new bool[MaxSlots];                //長押し状態のフラグ
    private bool[] _effectPlaying = new bool[MaxSlots];            //エフェクト再生中フラグ
    public bool _isDeath = false;                                  //死亡状態フラグ

    private readonly float _LOWER_LIMIT_MP = DefaultLowerLimitMP;  //インスタンス用の下限

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public PlayerMagicAttack() {
        InitializeLists();
        _isDeath = false;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void InitializeLists() {
        // 装備リストを初期化
        _equippedMagics = new List<eMagicType>(MaxSlots);
        for (int i = 0; i < MaxSlots; i++) {
            _equippedMagics.Add(eMagicType.Invalid);
        }

        // 取得済み魔法リストを初期化
        _acquiredMagics = new List<eMagicType>();
    }
    /// <summary>
    /// 指定スロット番号が有効か判定する。
    /// </summary>
    /// <param name="slotIndex">スロットインデックス</param>
    /// <returns>有効なら true</returns>
    private bool IsValidSlot(int slotIndex) {
        return slotIndex >= 0 && slotIndex < MaxSlots;
    }

    /// <summary>
    /// 指定スロットの SpawnPoint が割り当てられているかをチェック
    /// <param name="slotIndex"></param>
    /// <returns></returns>
    private bool EnsureSpawnPointExists(int slotIndex) {
        if (!IsValidSlot(slotIndex)) return false;
        var sp = _magicSpawnPos[slotIndex];
        if (sp == null) {
            return false;
        }
        return true;
    }

    /// <summary>
    /// 指定スロットに発射位置設定する。
    /// 受け取った GameObject は初期状態で inactive にする
    /// </summary>
    /// <param name="slotIndex">スロット番号（0~MaxSlots-1）</param>
    /// <param name="position">発射位置の GameObject</param>
    public void SetMagicSpawnPosition(int slotIndex, GameObject position) {
        if (!IsValidSlot(slotIndex)) return;
        _magicSpawnPos[slotIndex] = position;
        if (_magicSpawnPos[slotIndex] != null) {
            _magicSpawnPos[slotIndex].SetActive(false);
        }
    }

    /// <summary>
    /// 長押し状態フラグを設定する
    /// </summary>
    /// <param name="slotIndex">スロット番号</param>
    /// <param name="flag">長押し中フラグ</param>
    public void SetCastingFlag(int slotIndex, bool flag) {
        if (!IsValidSlot(slotIndex)) return;
        _isCasting[slotIndex] = flag;
    }

    /// <summary>
    /// 指定スロットの魔法をリクエストする。
    /// </summary>
    public void RequestAttack(int slotIndex) {
        // 範囲チェック
        if (!IsValidSlot(slotIndex)) return;

        // 入れ替え中なら入れ替え処理を優先
        if (_pendingMagic != eMagicType.Invalid) {
            ConfirmReplaceMagic(slotIndex);
            return;
        }

        // 現在の MP を取得
        float currentMP = CharacterUtility.GetPlayerCurrentMP();

        // 現状の装備魔法取得
        var magicType = _equippedMagics[slotIndex];
        if (magicType == eMagicType.Invalid) return;

        // MPが下限以下であれば魔法解除
        if (currentMP <= _LOWER_LIMIT_MP) {
            RequestCancelMagic(slotIndex);
            return;
        }

        // 死亡状態なら何もしない
        if (_isDeath) return;

        // SpawnPointがあるか確認
        if (!EnsureSpawnPointExists(slotIndex)) return;

        GameObject spawnPoint = _magicSpawnPos[slotIndex];

        // 魔法発動
        CreateMagic(eSideType.PlayerSide, magicType, spawnPoint);

        // 見た目の本を表示
        spawnPoint.SetActive(true);

        // MP残量厳密チェック
        if (currentMP <= _LOWER_LIMIT_MP) {
            RequestCancelMagic(slotIndex);
            return;
        }
    }

    /// <summary>
    /// 入れ替え処理
    /// </summary>
    public void ConfirmReplaceMagic(int slotIndex) {
        if (!IsValidSlot(slotIndex)) return;
        if (_pendingMagic == eMagicType.Invalid) return;

        // 発動中ならキャンセルしてから入れ替える
        var currentMagic = _equippedMagics[slotIndex];
        if (currentMagic != eMagicType.Invalid) {
            RequestCancelMagic(slotIndex);
        }

        // 入れ替え（既存仕様）
        ReplaceMagic(slotIndex, _pendingMagic);
        _pendingMagic = eMagicType.Invalid;
        isPendingMagic = false;

        // UI 関連
        SetMagicUI.Instance.CloseChangeMagicUI();

        // 防御魔法を常に展開
        MagicReset(eSideType.PlayerSide, eMagicType.Defense);
    }

    /// <summary>
    /// スロットの魔法発射を解除する
    /// </summary>
    public void RequestCancelMagic(int slotIndex) {
        if (!IsValidSlot(slotIndex)) return;

        var magicType = _equippedMagics[slotIndex];
        if (magicType == eMagicType.Invalid) return;

        GameObject spawnPoint = _magicSpawnPos[slotIndex];
        if (spawnPoint != null) {
            spawnPoint.SetActive(false);
        }

        // 魔法解除
        MagicReset(eSideType.PlayerSide, magicType);

        if (spawnPoint != null) {
            spawnPoint.SetActive(false);
        }

        // エフェクト再生フラグリセット
        _effectPlaying[slotIndex] = false;

        // 死亡時の追加解除
        if (_isDeath) {
            MagicReset(eSideType.PlayerSide, magicType);
        }
    }

    /// <summary>
    /// 魔法の最初の発動を始める
    /// </summary>
    public void StartCasting(int slotIndex) {
        if (!IsValidSlot(slotIndex)) return;

        var magicType = _equippedMagics[slotIndex];
        if (magicType == eMagicType.Invalid) return;

        float currentMP = CharacterUtility.GetPlayerCurrentMP();
        if (currentMP <= _LOWER_LIMIT_MP) return;

        if (_effectPlaying[slotIndex]) return;

        if (!EnsureSpawnPointExists(slotIndex)) return;
        GameObject spawnPoint = _magicSpawnPos[slotIndex];

        // エフェクト
        EffectManager.Instance.PlayEffect(eEffectType.Book, spawnPoint.transform.position).Forget();

        _effectPlaying[slotIndex] = true;
    }

    /// <summary>
    /// 解析魔法リクエストする。
    /// </summary>
    public void RequestAnalysis() {
        if (_isDeath) return;
        AnalysisMagicActivate();
    }

    /// <summary>
    /// 1フレーム分の魔法発射処理
    /// </summary>
    public async UniTask MagicUpdate() {
        var player = CharacterUtility.GetPlayer();
        if (player == null) return;
        if (_isDeath) return;

        for (int i = 0; i < _equippedMagics.Count; i++) {
            float currentMP = CharacterUtility.GetPlayerCurrentMP();

            if (currentMP <= _LOWER_LIMIT_MP) {
                RequestCancelMagic(i);
                continue;
            }

            // 長押しされているなら発射
            if (_isCasting[i]) {
                RequestAttack(i);
            }
        }

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 取得した魔法を所持リストに保存
    /// </summary>
    public static void SetMagicStorageSlot(eMagicType magicType) {
        _acquiredMagics.Add(magicType);

        // 空きスロットがあれば自動装備
        for (int i = 0; i < _equippedMagics.Count; i++) {
            if (_equippedMagics[i] == eMagicType.Invalid) {
                _equippedMagics[i] = magicType;
                SetMagicUI.Instance.UpdateMagicUI();
                return;
            }
        }

        // 空きが無ければ入れ替え待ちにする
        _pendingMagic = magicType;
        isPendingMagic = true;
        SetMagicUI.Instance.OpenChangeMagicUI();

        // 既存仕様どおり、防御魔法を即展開
        CreateMagic(eSideType.PlayerSide, eMagicType.Defense);
    }

    /// <summary>
    /// 取得済み魔法リストを返す
    /// </summary>
    public static List<eMagicType> GetMagicStorageSlot() {
        return _acquiredMagics;
    }

    /// <summary>
    /// 指定スロットを新しい魔法に置き換える。
    /// </summary>
    public void ReplaceMagic(int slotIndex, eMagicType newMagic) {
        if (!IsValidSlot(slotIndex)) return;
        _equippedMagics[slotIndex] = newMagic;
        SetMagicUI.Instance.UpdateMagicUI();
    }

    /// <summary>
    /// 魔法一覧 UI を開く
    /// </summary>
    public void OpenMagicUI() {
        SetMagicUI.Instance.OpenUI();
    }

    /// <summary>
    /// 魔法一覧 UI を閉じる
    /// </summary>
    public void CloseMagicUI() {
        SetMagicUI.Instance.CloseUI();
    }

    /// <summary>
    /// 現在装備している魔法一覧を取得
    /// </summary>
    public static List<eMagicType> GetEquippedMagicList() {
        return _equippedMagics;
    }

    /// <summary>
    /// 内部状態を初期化して片付ける。
    /// </summary>
    public void ResetState() {
        _isDeath = false;
    }

    /// <summary>
    /// 魔法のリセット
    /// </summary>
    public void ResetMagic() {
        InitializeLists();
        SetMagicUI.Instance.ResetMagicUI();
    }

}