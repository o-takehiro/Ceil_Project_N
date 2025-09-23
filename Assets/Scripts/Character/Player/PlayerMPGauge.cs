using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMPGauge : MenuBase {
    [SerializeField]
    private Slider _mpSlider = null;
    public CancellationToken _token;

    public Slider GetSlider() {
        return _mpSlider;
    }


    public override async UniTask Open() {
        _token = this.GetCancellationTokenOnDestroy();

        await base.Open();
        _mpSlider.value = 1.0f;

        while (!CharacterUtility.GetPlayer().isDead) {
            await UniTask.DelayFrame(1, 0, _token);

        }

        await Close();

    }
}
