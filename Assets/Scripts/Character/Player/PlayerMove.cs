
using Cysharp.Threading.Tasks;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

/// <summary>
/// プレイヤーの移動管理
/// </summary>
[RequireComponent(typeof(CharacterController))]
public sealed class PlayerMove : MonoBehaviour {
    // メインカメラ
    [SerializeField] private Camera _targetCamera;

    
    private CharacterController _controller;    // CharacterController
    private PlayerCharacter _character;         // PlayerCharacter.cs

    /// <summary>
    /// InputSystemの移動用コールバック
    /// </summary>
    /// <param name="ctx"></param>
    public void OnMove(InputAction.CallbackContext ctx)
        => _character?.SetMoveInput(ctx.ReadValue<Vector2>());

    /// <summary>
    /// InputSystemのジャンプ用コールバック
    /// </summary>
    /// <param name="ctx"></param>
    public void OnJump(InputAction.CallbackContext ctx) {
        if (ctx.performed) _character?.RequestJump();
    }
    
    /// <summary>
    /// InputSystemの攻撃用コールバック
    /// </summary>
    /// <param name="ctx"></param>
    public void OnAttack(InputAction.CallbackContext ctx) {

    }

    // 初期化
    private async void Start() {
        _controller = GetComponent<CharacterController>();
        if (_targetCamera == null) _targetCamera = Camera.main;

        // 既に付いているコンポーネントを取得
        _character = GetComponent<PlayerCharacter>();
        if (_character == null) {
            // 付いていなかった場合だけ追加
            _character = gameObject.AddComponent<PlayerCharacter>();
        }
        _character.Initialize(_controller, transform, _targetCamera, this);

        // UniTaskのキャンセルを行うときのハンドリング
        try {
            // 非同期ループ開始
            await _character.PlayerMainLoop(this.GetCancellationTokenOnDestroy());
        }
        catch (OperationCanceledException) {

        }
    }


    public void ApplyMovement(Vector3 delta) => _controller.Move(delta);
    public void ApplyRotation(Quaternion rot) => transform.rotation = rot;
}