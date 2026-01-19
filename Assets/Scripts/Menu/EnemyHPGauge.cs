using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPGauge : MenuBase {
    // HPゲージ
    [SerializeField]
    private Slider _hpSlider = null;

    /// <summary>
    /// スライダーの取得
    /// </summary>
    /// <returns></returns>
    public Slider GetSlider() {
        return _hpSlider;
    }
    /// <summary>
    /// 親オブジェクトの取得
    /// </summary>
    /// <returns></returns>
    public GameObject GetCanvas() {
        return gameObject;
    }
}
