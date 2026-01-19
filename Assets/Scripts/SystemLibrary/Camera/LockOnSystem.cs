/*
 *  @file   LockOnSystem
 *  @author oorui
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ロックオンの仕様
/// </summary>
public class LockOnSystem {
    // 現在のロックオン対象
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

    /// <summary>
    /// ロックオン中のターゲットの取得
    /// </summary>
    public Transform GetLockOnTarget() {
        return _currentLockTarget;
    }
}