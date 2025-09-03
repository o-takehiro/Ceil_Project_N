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
    [SerializeField] private Camera _targetCamera;   // 入力に使うカメラ（Inspector で指定可）

    private Rigidbody _rigidbody;       // Rigidbody（物理挙動用）
    private Animator _animator;         // Animator（アニメーション制御用）
    private PlayerCharacter _character; // プレイヤーキャラクター本体

    public void OnMove(InputAction.CallbackContext ctx) {
        if (_character == null) return;
        _character.SetMoveInput(ctx.ReadValue<Vector2>());
    }

    public void OnJump(InputAction.CallbackContext ctx) {
        if (_character == null) return;
        if (ctx.performed) _character.RequestJump();
    }

    public void OnAttack(InputAction.CallbackContext ctx) {
        if (_character == null) return;
        if (ctx.performed) _character.RequestAttack();
    }

    public void OnLookOn(InputAction.CallbackContext ctx) {
        if (_character == null) return;
        if (ctx.performed) _character.RequestLookOn();
    }

    /// <summary>
    /// 初期化処理
    /// - Rigidbody / Animator を取得
    /// - PlayerCharacter を準備
    /// - PlayerCharacter のメインループを開始
    /// </summary>
    private async void Start() {
        // Rigidbody を取得
        _rigidbody = GetComponent<Rigidbody>();

        // カメラを設定（未指定なら MainCamera を取得）
        if (_targetCamera == null) _targetCamera = Camera.main;

        // Animator を取得（必須コンポーネント）
        _animator = GetComponentInChildren<Animator>();
        if (_animator == null) {
            Debug.LogError("Animator が見つかりません。プレイヤーに Animator コンポーネントを追加してください。");
            return;
        }

        // PlayerCharacter を取得（なければ追加）
        _character = GetComponent<PlayerCharacter>();
        if (_character == null) {
            _character = gameObject.AddComponent<PlayerCharacter>();
        }

        // PlayerCharacter 初期化
        _character.Initialize(_rigidbody, transform, _targetCamera, this, _animator);

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
