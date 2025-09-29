using Cysharp.Threading.Tasks;
using System;
using System.Transactions;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 入力管理クラス
/// </summary>
public sealed class PlayerInput : MonoBehaviour {
    [SerializeField] private Camera _targetCamera;      // 入力に使うカメラ

    private Rigidbody _rigidbody;                       // Rigidbody
    private Animator _animator;                         // Animator
    private PlayerCharacter _character;                 // プレイヤーキャラクター本体
    public bool CanReceiveInput { get; set; } = true;   // 入力可能か

    /// <summary>
    /// 移動入力の取得
    /// </summary>
    /// <param name="ctx"></param>
    public void OnMove(InputAction.CallbackContext ctx) {
        if (_character == null) return;
        _character.SetMoveInput(ctx.ReadValue<Vector2>());
    }

    /// <summary>
    /// ジャンプ入力の取得
    /// </summary>
    /// <param name="ctx"></param>
    public void OnJump(InputAction.CallbackContext ctx) {
        if (_character == null) return;
        if (!CanReceiveInput) return;
        if (ctx.performed) _character.RequestJump();
    }

    /// <summary>
    /// 攻撃入力の取得
    /// </summary>
    /// <param name="ctx"></param>
    public void OnAttack(InputAction.CallbackContext ctx) {
        if (_character == null) return;
        if (!CanReceiveInput) return;
        if (ctx.performed) _character.RequestAttack();
    }

    /// <summary>
    /// ロックオン入力の取得
    /// </summary>
    /// <param name="ctx"></param>
    public void OnLookOn(InputAction.CallbackContext ctx) {
        if (_character == null || !CanReceiveInput) return;
        if (ctx.performed) _character.RequestLookOn();
    }

    // 魔法スロット １
    public void OnCastSlot1(InputAction.CallbackContext ctx) {
        if (!CanReceiveInput) return;

        // 長押し
        if (ctx.performed) {
            _character.RequestSetCastingFlag(0, true);
            _character.RequestStartCasting(0);
            _character.RequestReplaceMagic(0);
        }
        else if (ctx.canceled) {
            _character.RequestSetCastingFlag(0, false);
            _character.RequestCastMagicEnd(0);
        }
    }
    // 魔法スロット　２
    public void OnCastSlot2(InputAction.CallbackContext ctx) {
        if (!CanReceiveInput) return;

        // 長押し
        if (ctx.performed) {
            _character.RequestSetCastingFlag(1, true);
            _character.RequestStartCasting(1);
            _character.RequestReplaceMagic(1);
        }
        else if (ctx.canceled) {
            _character.RequestSetCastingFlag(1, false);
            _character.RequestCastMagicEnd(1);
        }
    }
    // 魔法スロット　３
    public void OnCastSlot3(InputAction.CallbackContext ctx) {
        if (!CanReceiveInput) return;

        // 長押し
        if (ctx.performed) {
            _character.RequestSetCastingFlag(2, true);
            _character.RequestStartCasting(2);
            _character.RequestReplaceMagic(2);
        }
        else if (ctx.canceled) {
            _character.RequestSetCastingFlag(2, false);
            _character.RequestCastMagicEnd(2);
        }
    }
    // 魔法スロット　４
    public void OnCastSlot4(InputAction.CallbackContext ctx) {
        if (!CanReceiveInput) return;

        // 長押し
        if (ctx.performed) {
            _character.RequestSetCastingFlag(3, true);
            _character.RequestStartCasting(3);
            _character.RequestReplaceMagic(3);
        }
        else if (ctx.canceled) {
            _character.RequestSetCastingFlag(3, false);
            _character.RequestCastMagicEnd(3);
        }
    }

    /// <summary>
    /// 解析魔法の入力受付
    /// </summary>
    /// <param name="ctx"></param>
    public void OnAnalysis(InputAction.CallbackContext ctx) {
        if (!CanReceiveInput) return;
        if (ctx.performed) {
            _character.RequestAnalysis();
        }
        else if (ctx.canceled) {

        }
    }

    /// <summary>
    /// 魔法所持リストの表示切り替えの受付
    /// </summary>  
    /// <param name="ctx"></param>
    public void OnMagicOpen(InputAction.CallbackContext ctx) {
        if (!CanReceiveInput) return;
        if (ctx.performed) {
            // 魔法リスト表示
            _character.RequestOpenMagicUI();
        }
        else if (ctx.canceled) {
            // 魔法リスト非表示
            _character.RequestCloceMagicUI();
        }
    }


    /// <summary>
    /// 準備処理
    /// </summary>
    private void Start() {
        // Rigidbody を取得（移動用に必要）
        _rigidbody = GetComponent<Rigidbody>();
        if (_targetCamera == null) _targetCamera = Camera.main;

        // Animatorの取得
        _animator = GetComponentInChildren<Animator>();
        if (_animator == null) return;

        // PlayerCharacter を取得 or 追加
        _character = GetComponent<PlayerCharacter>();
        if (_character == null) {
            _character = gameObject.AddComponent<PlayerCharacter>();
        }

    }

}
