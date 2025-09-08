using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class EnemyHPSliderRotation : MonoBehaviour {
    private void Update() {
        LookatPlayer();  
    }

    private void LookatPlayer() {
        if (GetPlayer() == null) return;

        // プレイヤーへの方向
        Vector3 direction = GetPlayerPosition() - transform.position;

        // 高さを無視して水平回転だけにする
        direction.y = 0;

        if (direction.sqrMagnitude < 0.001f)
            return; // ゼロ除け

        // そのまま回転を適用（補間なし、瞬時に回転）
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
