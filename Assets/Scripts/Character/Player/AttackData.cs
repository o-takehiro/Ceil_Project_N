using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 核攻撃事のデータ
/// </summary>
public class AttackData {
    public string AnimationName { get; set; }           // アニメーションの名前
    public float Damage { get; set; }                   // 与えるダメージ
    public int ColliderActiveDurationMs { get; set; }   // コライダー有効時間 (ミリ秒)
    public int PostDelayMs { get; set; }                // 攻撃後の硬直時間 (ミリ秒)
    public int HealMP { get; set; }                     // MPの回復量

    public AttackData(string anim, float dmg, int colliderMs, int delayMs, int healMP) {
        AnimationName = anim;
        Damage = dmg;
        ColliderActiveDurationMs = colliderMs;
        PostDelayMs = delayMs;
        HealMP = healMP;
    }
}
