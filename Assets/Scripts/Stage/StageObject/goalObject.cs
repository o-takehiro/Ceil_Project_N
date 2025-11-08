/*
 *  @fili   goalObject
 *  @author     oorui
 */

using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// ステージに配置するオブジェクト、ゴール
/// </summary>
public class goalObject : StageObjectBase {
    // 自身のCollider
    private Collider _collider;
    // エフェクト再生用のポジション
    [SerializeField] private GameObject _goalObjectRoot;
    private bool _hasPlayedFocus = false;
    public bool IsPlayerReachedGoal { get; private set; }

    /// <summary>
    /// 準備
    /// </summary>
    public override void SetUp() {
        base.SetUp();
        // コライダーを取得
        if (_collider == null) {
            _collider = GetComponent<Collider>();
        }
        // 初期生成時は判定、表示はしない
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

    /// <summary>
    /// ゴール判定
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        IsPlayerReachedGoal = true;
    }

    /// <summary>
    /// 片付け処理
    /// </summary>
    public override void Teardown() {
        base.Teardown();
        CharacterUtility.UnusePlayer();
        _goalObjectRoot.SetActive(false);
        GetComponent<Collider>().enabled = false;
        _hasPlayedFocus = false;
        IsPlayerReachedGoal = false; // リセット
    }
}
