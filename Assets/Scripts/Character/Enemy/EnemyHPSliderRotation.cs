using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class EnemyHPSliderRotation : MonoBehaviour {
    // プレイヤーカメラ
    private Transform playerCamera;

    private void Start() {
        playerCamera = Camera.main.transform;
    }

    private void LateUpdate() {
        if (GetEnemy() == null)
            return;
        LookAtCamera();
    }

    private void LookAtCamera() {
        Vector3 pos = transform.position;
        Vector3 targetPos = playerCamera.position;

        // 高さを固定（y座標を同じにする）
        targetPos.y = pos.y;

        Vector3 dir = targetPos - pos;
        if (dir.sqrMagnitude < 0.001f)
            return;

        // ワールド回転を直接適用（親の回転は無視される）
        transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);

        // 裏返るときは180°補正
        transform.Rotate(0, 180f, 0);
    }
}
