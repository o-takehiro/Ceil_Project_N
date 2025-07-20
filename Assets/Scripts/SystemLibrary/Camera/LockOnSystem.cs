using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockOnSystem : MonoBehaviour {
    // 現在のロック御大将
    private Transform _currentLockTarget;

    /// <summary>
    /// 現在のロックオン対象
    /// </summary>
    public Transform CurrentTarget => _currentLockTarget;

    /// <summary>
    /// ターゲットをロックオン
    /// </summary>
    public void LockOn(Transform target) {
        _currentLockTarget = target;
    }

    /// <summary>
    /// ロックオンを解除
    /// </summary>
    public void Unlock() {
        _currentLockTarget = null;
    }

    /// <summary>
    /// ロックオン中かどうか
    /// </summary>
    public bool IsLockedOn() {
        return _currentLockTarget != null;
    }
}
