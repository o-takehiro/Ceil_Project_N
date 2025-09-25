using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


/// <summary>
/// 魔法切り替えクラス
/// </summary>
public class MagicReplaceUI : MenuBase {
    public static MagicReplaceUI Instance { get; private set; } = null;

    private eMagicType _pendingMagic;

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        Instance = this;
        gameObject.SetActive(false);

        await UniTask.CompletedTask;
    }

    /// <summary>
    /// 新魔法を受け取ったときにUIを開く
    /// </summary>
    public void ShowReplaceChoice(eMagicType newMagic) {
        _pendingMagic = newMagic;
        gameObject.SetActive(true); // UI表示ON
        Debug.Log($"新しい魔法 {_pendingMagic} を取得！どのスロットと入れ替えますか？");
    }

    /// <summary>
    /// スロットを選択（ボタン入力で呼ばれる）
    /// </summary>
    public void OnReplaceSlot(int slotIndex) {
        var player = CharacterUtility.GetPlayer();
        if (player == null) return;

        player.GetComponent<PlayerCharacter>()
              .GetMagicController()
              .ReplaceMagic(slotIndex, _pendingMagic);

        _pendingMagic = eMagicType.Invalid;
        gameObject.SetActive(false); // UI閉じる
    }


}
