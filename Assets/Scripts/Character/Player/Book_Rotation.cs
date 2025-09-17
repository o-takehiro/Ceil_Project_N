using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Book_Rotation : MonoBehaviour {
    private Camera _camera;

    void Start() {
        // MainCameraタグのついたカメラを一度だけ取得
        _camera = Camera.main;
    }

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
