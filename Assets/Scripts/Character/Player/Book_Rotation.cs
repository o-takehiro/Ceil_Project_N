/*
 *  @file   Book_Rotation
 *  @author oorui
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book_Rotation : MonoBehaviour {
    private Camera _camera;

    /// <summary>
    /// 初期化
    /// </summary>
    void Start() {
        // カメラの取得
        _camera = Camera.main;
        if (_camera == null) return;
    }


    /// <summary>
    /// 更新処理
    /// </summary>
    void LateUpdate() {
        if (_camera == null) return;

        // カメラのforwardを取得
        Vector3 cameraForward = _camera.transform.forward;

        // 上方向を固定する場合はY成分を消す
        cameraForward.y = 0f;

        if (cameraForward.sqrMagnitude > 0.001f) {
            transform.rotation = Quaternion.LookRotation(cameraForward.normalized);
        }
    }
}
