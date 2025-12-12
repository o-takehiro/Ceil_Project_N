using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 入力管理クラス
/// </summary>
public sealed class PlayerInput : MonoBehaviour {
    [SerializeField] private Camera _targetCamera;      // 入力時に参照するカメラ

    private Rigidbody _rigidbody;                       // 移動処理に使用する Rigidbody
    private Animator _animator;                         // プレイヤーの Animator
    private PlayerCharacter _character;                 // プレイヤーキャラクター本体

    public bool CanReceiveInput { get; set; } = true;   // 入力受付の可否


    /// <summary>
    /// 番号毎のボタンに対応した魔法
    /// </summary>
    private void HandleMagicSlot(InputAction.CallbackContext ctx, int slotIndex) {
        // ボタンが離された瞬間
        if (ctx.canceled) {
            _character.RequestSetCastingFlag(slotIndex, false);
            _character.RequestCastMagicEnd(slotIndex);
            return;
        }

        // 入力不可状態なら何もしない
        if (!CanReceiveInput) return;

        // 長押し処理
        if (ctx.performed) {
            _character.RequestSetCastingFlag(slotIndex, true);
            _character.RequestStartCasting(slotIndex);
            _character.RequestReplaceMagic(slotIndex);
        }
    }

    /// <summary>
    /// 移動入力を受け取る
    /// </summary>
    public void OnMove(InputAction.CallbackContext ctx) {
        if (_character == null) return;
        _character.SetMoveInput(ctx.ReadValue<Vector2>());
    }

    /// <summary>
    /// ジャンプ入力を受け取る
    /// </summary>
    public void OnJump(InputAction.CallbackContext ctx) {
        if (!CanReceiveInput || _character == null) return;

        if (ctx.performed) {
            _character.RequestJump();
        }
    }

    /// <summary>
    /// 攻撃入力を受け取る
    /// </summary>
    public void OnAttack(InputAction.CallbackContext ctx) {
        if (!CanReceiveInput || _character == null) return;

        if (ctx.performed) {
            _character.RequestAttack();
        }
    }

    /// <summary>
    /// ロックオン切り替え入力
    /// </summary>
    public void OnLookOn(InputAction.CallbackContext ctx) {
        if (!CanReceiveInput || _character == null) return;

        if (ctx.performed) {
            _character.RequestLookOn();
        }
    }

    /// <summary>
    /// 4つの魔法のボタン入力
    /// </summary>
    /// <param name="ctx"></param>
    public void OnCastSlot1(InputAction.CallbackContext ctx) => HandleMagicSlot(ctx, 0);
    public void OnCastSlot2(InputAction.CallbackContext ctx) => HandleMagicSlot(ctx, 1);
    public void OnCastSlot3(InputAction.CallbackContext ctx) => HandleMagicSlot(ctx, 2);
    public void OnCastSlot4(InputAction.CallbackContext ctx) => HandleMagicSlot(ctx, 3);

    /// <summary>
    /// 解析魔法入力
    /// </summary>
    public void OnAnalysis(InputAction.CallbackContext ctx) {
        if (!CanReceiveInput || _character == null) return;

        if (ctx.performed) {
            // 解析魔法使用
            _character.RequestAnalysis();
        }
    }

    /// <summary>
    /// 魔法リスト UI の表示
    /// </summary>
    public void OnMagicOpen(InputAction.CallbackContext ctx) {
        if (!CanReceiveInput || _character == null) return;


        if (ctx.performed) {
            // 魔法リストUIを開く
            _character.RequestOpenMagicUI();
        }
        else if (ctx.canceled) {
            // 魔法リストUIを閉じる
            _character.RequestCloceMagicUI();
        }
    }

    /// <summary>
    ///  初期化
    /// </summary>
    private void Start() {
        // Rigidbody
        _rigidbody = GetComponent<Rigidbody>();

        // カメラ
        if (_targetCamera == null) _targetCamera = Camera.main;

        // アニメーター取得
        _animator = GetComponentInChildren<Animator>();

        // PlayerCharacter を取得
        _character = GetComponent<PlayerCharacter>();
        if (_character == null) {
            _character = gameObject.AddComponent<PlayerCharacter>();
        }
    }
}
