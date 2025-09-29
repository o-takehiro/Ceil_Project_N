using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public class goalObject : StageObjectBase {
    // 自身のCollider
    private Collider _collider;
    // エフェクト再生用のポジション
    [SerializeField] private GameObject _goalObjectRoot;
    private bool _hasPlayedFocus = false;
    public bool IsPlayerReachedGoal { get; private set; }

    public override void SetUp() {
        base.SetUp();
        if (_collider == null) {
            _collider = GetComponent<Collider>();
        }
        _goalObjectRoot.SetActive(false);
        _collider.enabled = false;
        IsPlayerReachedGoal = false;
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    protected override async void OnUpdate() {

        if (!_hasPlayedFocus && StageManager.Instance.GetCurrentStageClear()) {
            _goalObjectRoot.SetActive(true);
            GetComponent<Collider>().enabled = true;
            _hasPlayedFocus = true;

            // プレイヤーのMPを回復
            CharacterUtility.ToPlayerAddMP((int)CharacterUtility.GetPlayer().maxMP);

            // SE再生
            SoundManager.Instance.PlaySE(21);
            UniTask task = MagicManager.instance.MagicReset(eSideType.PlayerSide, eMagicType.Defense);
            // カメラモーション
            await CameraManager.Instance.FocusOnObject(_goalObjectRoot.transform);
            //CharacterUtility.ResumePlayer();
        }

    }

    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        IsPlayerReachedGoal = true;
    }

    public override void Teardown() {
        base.Teardown();
        CharacterUtility.UnusePlayer();
        _goalObjectRoot.SetActive(false);
        GetComponent<Collider>().enabled = false;
        _hasPlayedFocus = false;
        IsPlayerReachedGoal = false; // リセット
    }
}
