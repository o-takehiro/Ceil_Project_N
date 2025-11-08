/*
 *  @fili   StartObject
 *  @author     oorui
 */

using Cysharp.Threading.Tasks;
using UnityEngine;

using static CharacterUtility;
/// <summary>
/// ステージに配置するオブジェクト、スタート
/// </summary>
public class StartObject : StageObjectBase {

    // 出現場所を指定
    [SerializeField]
    private GameObject PlayerStartPos = null;
    [SerializeField]
    private GameObject EnemyStartPos = null;

    /// <summary>
    /// 準備
    /// </summary>
    public override async void SetUp() {
        base.SetUp();
        // プレイヤーを生成
        CreatePlayer();
        // 敵を生成
        CreateEnemy();
        // 数フレーム松
        await UniTask.DelayFrame(1);
    }
    protected override void OnUpdate() {
    }

    /// <summary>
    /// プレイヤーを自身の位置に移動
    /// </summary>
    private void CreatePlayer() {
        // プレイヤー生成
        UsePlayer(0);
        // 座標を設定
        CharacterManager.instance.SetUsePlayerPosition(PlayerStartPos.transform.position);
    }

    /// <summary>
    /// 敵を自身の位置に移動
    /// </summary>
    private void CreateEnemy() {
        if (GetEnemy() != null) return;
        // 現在のステージを取得
        eStageState stage = StageManager.Instance.GetCurrentStageState();

        ///  ステージ毎に切り替える
        switch (stage) {

            case eStageState.Tutorial:
                UseEnemy(eEnemyType.TutorialEnemy);
                break;
            case eStageState.Stage1:
                UseEnemy(eEnemyType.Stage1Enemy);
                break;
            case eStageState.Stage2:
                UseEnemy(eEnemyType.Stage2Enemy);
                break;
            case eStageState.Stage3:
                UseEnemy(eEnemyType.Stage3Enemy);
                break;
        }

        // 座標を設定
        CharacterManager.instance.SetUseEnemyPosition(EnemyStartPos.transform.position);
    }

}
