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
    private bool[] _isCasting = new bool[4];            // 各スロットの押下状態
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
        if (ctx.performed) _character.RequestJump();
    }

    /// <summary>
    /// 攻撃入力の取得
    /// </summary>
    /// <param name="ctx"></param>
    public void OnAttack(InputAction.CallbackContext ctx) {
        if (_character == null || !CanReceiveInput) return;
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
        // 現在のMP量を取得
        float currentMP = CharacterUtility.GetPlayerCurrentMP();

        if (!CanReceiveInput) return;

        if (ctx.performed) {
            // MPが0より大きいときだけ発動
            if (currentMP > 0.0f) {
                _character.RequestCastMagic(0);
            }
            else {
                _character.RequestCastMagicEnd(0);
            }
        }
        else if (ctx.canceled) {
            // ボタンを離したとき
            _character.RequestCastMagicEnd(0);
        }
    }
    // 魔法スロット　２
    public void OnCastSlot2(InputAction.CallbackContext ctx) {
        float currentMP = CharacterUtility.GetPlayerCurrentMP();

        if (!CanReceiveInput) return;

        if (ctx.performed) {
            // MPが0より大きいときだけ発動
            if (currentMP > 0.0f) {
                _character.RequestCastMagic(1);
            }
            else {
                _character.RequestCastMagicEnd(1);
            }
        }
        else if (ctx.canceled) {
            // ボタンを離したとき
            _character.RequestCastMagicEnd(1);
        }
    }
    // 魔法スロット　３
    public void OnCastSlot3(InputAction.CallbackContext ctx) {
        float currentMP = CharacterUtility.GetPlayerCurrentMP();

        if (!CanReceiveInput) return;

        if (ctx.performed) {
            // MPが0より大きいときだけ発動
            if (currentMP > 0.0f) {
                _character.RequestCastMagic(2);
            }
            else {
                _character.RequestCastMagicEnd(2);
            }
        }
        else if (ctx.canceled) {
            // ボタンを離したとき
            _character.RequestCastMagicEnd(2);
        }
    }
    // 魔法スロット　４
    public void OnCastSlot4(InputAction.CallbackContext ctx) {
        float currentMP = CharacterUtility.GetPlayerCurrentMP();

        if (!CanReceiveInput) return;

        if (ctx.performed) {
            // MPが0より大きいときだけ発動
            if (currentMP > 0.0f) {
                _character.RequestCastMagic(3);
            }
            else {
                _character.RequestCastMagicEnd(3);
            }
        }
        else if (ctx.canceled) {
            // ボタンを離したとき
            _character.RequestCastMagicEnd(3);
        }
    }

    /// <summary>
    /// 解析魔法の入力受付
    /// </summary>
    /// <param name="ctx"></param>
    public void OnAnalysis(InputAction.CallbackContext ctx) {
        if (ctx.performed || !CanReceiveInput) {
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
