using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの入力を受け取り、PlayerCharacter に伝えるクラス
/// - 新InputSystem のコールバックに対応
/// - 移動、ジャンプ、攻撃、ロックオンなどを仲介する
/// </summary>
public sealed class PlayerInput : MonoBehaviour {
    [SerializeField] private Camera _targetCamera;   // 入力に使うカメラ

    private Rigidbody _rigidbody;       // Rigidbody
    private Animator _animator;         // Animator
    private PlayerCharacter _character; // プレイヤーキャラクター本体

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
        if (_character == null) return;
        if (ctx.performed) _character.RequestAttack();
    }

    /// <summary>
    /// ロックオン入力の取得
    /// </summary>
    /// <param name="ctx"></param>
    public void OnLookOn(InputAction.CallbackContext ctx) {
        if (_character == null) return;
        if (ctx.performed) _character.RequestLookOn();
    }

    // 魔法スロット １
    public void OnCastSlot1(InputAction.CallbackContext ctx) {
        if (ctx.performed) {
            // 押されたとき
            _character.RequestCastMagic(0);
        }
        else if (ctx.canceled) {
            // 離されたとき
            _character.RequestCastMagicEnd(0);
        }
    }
    // 魔法スロット　２
    public void OnCastSlot2(InputAction.CallbackContext ctx) {
        if (ctx.performed) {
            // 押されたとき
            _character.RequestCastMagic(1);
        }
        else if (ctx.canceled) {
            // 離されたとき
            _character.RequestCastMagicEnd(1);
        }
    }
    // 魔法スロット　３
    public void OnCastSlot3(InputAction.CallbackContext ctx) {
        if (ctx.performed) {
            // 押されたとき
            _character.RequestCastMagic(2);
        }
        else if (ctx.canceled) {
            // 離されたとき
            _character.RequestCastMagicEnd(2);
        }
    }
    // 魔法スロット　４
    public void OnCastSlot4(InputAction.CallbackContext ctx) {
        if (ctx.performed) {
            // 押されたとき
            _character.RequestCastMagic(3);
        }
        else if (ctx.canceled) {
            // 離されたとき
            _character.RequestCastMagicEnd(3);
        }
    }


    /// <summary>
    /// 準備処理
    /// </summary>
    private async void Start() {
        // Rigidbody を取得
        _rigidbody = GetComponent<Rigidbody>();
        // カメラを設定
        if (_targetCamera == null) _targetCamera = Camera.main;

        // Animatorの取得
        _animator = GetComponentInChildren<Animator>();
        if (_animator == null) return;

        // PlayerCharacterを取得
        _character = GetComponent<PlayerCharacter>();
        if (_character == null) {
            _character = gameObject.AddComponent<PlayerCharacter>();
        }

        // PlayerCharacter 初期化
        _character.InjectDependencies(_rigidbody, transform, _targetCamera, this, _animator);
        _character.Initialize();
        // UniTask のキャンセルを考慮してループ開始
        try {
            await _character.PlayerMainLoop(this.GetCancellationTokenOnDestroy());
        }
        catch (OperationCanceledException) {
            // Destroy 時にキャンセルされた場合は無視
        }
    }

    /// <summary>
    /// 移動処理（物理演算用）
    /// - targetPosition に補間移動する
    /// </summary>
    public void ApplyMovement(Vector3 targetPosition) {
        _rigidbody.MovePosition(targetPosition);
    }

    /// <summary>
    /// 回転処理（物理演算用）
    /// - rot に補間回転する
    /// </summary>
    public void ApplyRotation(Quaternion rot) {
        _rigidbody.MoveRotation(rot);

    }
}
