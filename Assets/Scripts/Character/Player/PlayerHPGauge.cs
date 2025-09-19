using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPGauge : MenuBase {

    [SerializeField]
    private Slider _hpSlider = null;

    public CancellationToken _token;

    public Slider GetSlider() {
        return _hpSlider;
    }


    public override async UniTask Open() {
        _token = this.GetCancellationTokenOnDestroy();

        await base.Open();
        _hpSlider.value = 1.0f;

        var player = CharacterUtility.GetPlayer();
        if (player == null) return;

        while (!player.isDead) {
            await UniTask.DelayFrame(1, 0, _token);

        }

        await Close();
    }

}
