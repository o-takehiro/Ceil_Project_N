
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// プレイヤーの移動管理
/// </summary>
[RequireComponent(typeof(CharacterController))]
public sealed class PlayerMove : MonoBehaviour {
    // メインカメラ
    [SerializeField] private Camera _targetCamera;

    private CharacterController _controller;
    private PlayerCharacter _character;      // ドメインクラス

    /// <summary>
    /// InputSystemのコールバック
    /// </summary>
    /// <param name="ctx"></param>
    public void OnMove(InputAction.CallbackContext ctx)
        => _character?.SetMoveInput(ctx.ReadValue<Vector2>());

    /// <summary>
    /// InputSystemのコールバック
    /// </summary>
    /// <param name="ctx"></param>
    public void OnJump(InputAction.CallbackContext ctx) {
        if (ctx.performed) _character?.RequestJump();
    }

    // 初期化
    private async void Start() {
        _controller = GetComponent<CharacterController>();
        if (_targetCamera == null) _targetCamera = Camera.main;

        // ドメインクラス生成
        _character = new PlayerCharacter(_controller, transform, _targetCamera, this);

        // UniTaskのキャンセルを行うときのハンドリング
        try {
            // 非同期ループ開始（破棄時に自動キャンセル）
            await _character.PlayerMainLoop(this.GetCancellationTokenOnDestroy());
        }
        catch (OperationCanceledException) {

        }
    }


    public void ApplyMovement(Vector3 delta) => _controller.Move(delta);
    public void ApplyRotation(Quaternion rot) => transform.rotation = rot;
}