using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPGauge : MenuBase {

    [SerializeField]
    private Slider _hpSlider = null;

    public Slider GetSlider() {
        return _hpSlider;
    }

    
    public override async UniTask Open() {
        await base.Open();
        _hpSlider.value = 1.0f;
    }

}
