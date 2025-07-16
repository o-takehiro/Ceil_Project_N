using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHPGauge : MenuBase {
    [SerializeField]
    private Slider _hpSlider = null;

    public Slider GetSlider() {
        return _hpSlider;
    }

    public GameObject GetCanvas() {
        return gameObject;
    }
}
