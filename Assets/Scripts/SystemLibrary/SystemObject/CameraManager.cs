using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SystemObject {
    // 自身への参照
    public static CameraManager Instance { get; private set; } = null;

    // カメラ
    private Camera _camera;
    // カメラオブジェクトの名前
    private const string _CAMERA_NAME = "Main Camera";

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    public override async UniTask Initialize() {
        Instance = this;
        // シーン上のカメラを探してキャッシュしておく
        _camera = GameObject.Find(_CAMERA_NAME).GetComponent<Camera>();
        await UniTask.CompletedTask;
    }
}
