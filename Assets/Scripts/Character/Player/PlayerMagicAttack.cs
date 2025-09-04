using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの魔法を撃つ処理
/// </summary>
public class PlayerMagicAttack {
    private readonly Animator _animator;    // Animator参照

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="animator"></param>
    public PlayerMagicAttack(Animator animator) {
        _animator = animator;
    }

    /// <summary>
    /// 魔法発射
    /// </summary>
    public void RequestAttack(int slotIndex) {
        Debug.Log("魔法発射");

    }

    /// <summary>
    /// 魔法発射解除
    /// </summary>
    /// <param name="slotIndex"></param>
    public void RequestCancelMagic(int slotIndex) {
        Debug.Log("魔法終了");
    }

    /// <summary>
    /// 1フレーム分の発射処理
    /// 非同期
    /// </summary>
    /// <returns></returns>
    public async UniTask MagicUpdate() {





        await UniTask.CompletedTask;
    }


}
