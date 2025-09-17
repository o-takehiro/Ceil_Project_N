using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMPGauge : MenuBase {
    [SerializeField]
    private Slider _mpSlider = null;

    public Slider GetSlider() {
        return _mpSlider;
    }


    public override async UniTask Open() {
        await base.Open();
        _mpSlider.value = 1.0f;
    }
}
