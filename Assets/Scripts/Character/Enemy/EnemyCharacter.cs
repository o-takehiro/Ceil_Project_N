/*
 * @file    EnemyCharacter.cs
 * @brief   敵キャラクター情報
 * @author  Seki
 * @date    2025/7/8
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using static CharacterUtility;
using static MagicUtility;
using static CommonModule;
using static CharacterMasterUtility;

public class EnemyCharacter : CharacterBase {
    // 敵のHPゲージ
    [SerializeField]
    protected GameObject enemyCanvas = null;
    [SerializeField]
    private List<EnemyAttackCollider> _attackColliderList = null;
    protected Slider enemyHPGauge = null;
    protected Animator enemyAnimator = null;
    public CharacterAIMachine<EnemyCharacter> myAI { get; protected set; } = null;
    public CharacterAIBase<EnemyCharacter> actionMachine { get; protected set; } = null;

    protected List<eMagicType> magicTypeList = null;

    private int _enemyAttackValue = -1;

    // 行動判断データ
    protected DecisionData decisionData;
    // 行動判断材料
    protected DecisionFactors factors;
    // 行動判断インターフェース
    protected IEnemyDecision decision = null;
    protected eEnemyActionType actionType = eEnemyActionType.Invalid;
    // 行動リスト
    protected List<int> actionList = null;
    // 現在の行動クラス
    protected IEnemyAction currentAction = null;
    // 一つ前の行動クラス
    protected IEnemyAction prevAction = null;
    // 移動速度
    protected float moveSpeed = -1;
    // クールタイム
    protected float coolTime = -1.0f;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public override void Initialize() {
        base.Initialize();
        decisionData = new DecisionData();
        actionList = new List<int>();
        factors = new DecisionFactors();
    }
    /// <summary>
    /// 準備前処理
    /// </summary>
    /// <param name="masterID"></param>
    public override void Setup(int masterID) {
        base.Setup(masterID);
        // 判断材料データの設定
        decisionData.SetupData(masterID);
        // キャラクターのデータ設定
        var masterData = GetCharacterMaster(masterID);
        SetupData(masterData);
        //現在の位置更新
        SetEnemyPosition(Vector3.zero);
        //一フレーム前の位置更新
        SetEnemyPrevPosition();
        //中心座標の更新
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
        //現在の回転更新
        SetEnemyRotation(Quaternion.identity);
    }
    public void SetupData(Entity_CharacterData.Param setData) {
        SetID(setData.ID);
        SetMaxHP(setData.HP);
        SetHP(setData.HP);
        SetRawAttack(setData.Attack);
        SetRawDefense(setData.Defense);
        SetMoveSpeed(setData.MoveSpeed);
        SetActionID(setData.ActionID);
    }
    public override void Teardown() {
        base.Teardown();
        enemyHPGauge.value = 1.0f;
    }
    /// <summary>
    /// プレイヤーの判別
    /// </summary>
    /// <returns></returns>
    public override bool isPlayer() {
        return false;
    }
    /// <summary>
    /// 死亡処理
    /// </summary>
    public override void Dead() {
        myAI.ChangeState(new EnemyAI008_Empty());
        enemyAnimator.SetBool("isDead", true);
        CancelAllEnemyMagic();
        SetAllActiveCollider(false);
        enemyHPGauge.gameObject.SetActive(false);
    }
    /// <summary>
    /// 移動速度の取得
    /// </summary>
    /// <returns></returns>
    public float GetMoveSpeed() {
        return moveSpeed;
    }
    /// <summary>
    /// 移動速度の設定
    /// </summary>
    /// <param name="setSpeed"></param>
    public void SetMoveSpeed(float setSpeed) {
        moveSpeed = setSpeed;
    }
    /// <summary>
    /// クールタイムフラグの設定
    /// </summary>
    /// <param name="setFlag"></param>
    public void SetIsCoolTime(bool setFlag) {
        factors.isCoolTime = setFlag;
    }
    /// <summary>
    /// 判断材料の更新
    /// </summary>
    public void UpdateDecisions() {
        float distance = GetPlayerToEnemyDistance();
        factors.isPlayerClose = decisionData.closePlayerDistance < distance;
        factors.isPlayerFar = decisionData.farPlayerDisance > distance;
        List<int> activeMagicList = decisionData.playerActiveMagic;
        if(IsEmpty(activeMagicList)) return;
        for (int i = 0, max = activeMagicList.Count; i < max; i++) {
            if(!GetMagicActive((int)eSideType.PlayerSide, (int)eMagicType.MiniBullet)) continue;

            factors.isPlayerActiveMagic[i] = true;
        }
    }

    protected void SetEnemyCanvas() {
        if (enemyHPGauge != null) return;

        enemyHPGauge = MenuManager.Instance.Get<EnemyHPGauge>().GetSlider();
    }

    protected virtual void SetupCanvasPosition(Vector3 setSize) {
        Vector3 canvasPos = Vector3.zero;
        SetEnemyCanvas();
        enemyHPGauge.transform.SetParent(enemyCanvas.transform, false);
        enemyHPGauge.transform.localPosition = Vector3.zero;
        enemyHPGauge.transform.localRotation = Quaternion.Euler(0, 180f, 0); // 毎回180度に確定
        enemyHPGauge.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 0.5f);
        enemyCanvas.transform.localScale = setSize;
        enemyCanvas.gameObject.SetActive(true);
        enemyHPGauge.gameObject.SetActive(true);
    }

    public CharacterAIBase<EnemyCharacter> GetActionMachine() {
        return actionMachine;
    }

    public Animator GetEnemyAnimator() {
        return enemyAnimator;
    }

    public Slider GetEnemySlider() {
        return enemyHPGauge;
    }

    public float GetEnemySliderValue() {
        return HP / maxHP;
    }

    public void SetActiveCollider(int setValue, bool setFlag) {
        if(_attackColliderList[setValue].gameObject.activeSelf == setFlag) return;
        _attackColliderList[setValue].gameObject.SetActive(setFlag);
    }
    private void SetAllActiveCollider(bool setFlag) {
        for (int i = 0, max = _attackColliderList.Count; i < max; i++) {
            GameObject colliderObject = _attackColliderList[i].gameObject;
            if (colliderObject.activeSelf == setFlag) continue;

            colliderObject.SetActive(setFlag);
        }
    }
    public int GetEnemyAttackValue() {
        return _enemyAttackValue;
    }
    public void SetEnemyAttackValue(int setValue) {
        _enemyAttackValue = setValue;
    }
    /// <summary>
    /// 指定した種類の魔法を取得
    /// </summary>
    /// <param name="magicType"></param>
    /// <returns></returns>
    public eMagicType GetEnemyMagicType(eMagicType magicType) {
        for (int i = 0, max = magicTypeList.Count; i < max; i++) {
            eMagicType magic = magicTypeList[i];
            if (magic != magicType) continue;

            return magic;
        }
        return eMagicType.Invalid;
    }
    /// <summary>
    /// リストに魔法の種類を追加
    /// </summary>
    /// <param name="magicType"></param>
    public void AddEnemyMagicList(eMagicType magicType) {
        magicTypeList.Add(magicType);
    }
    /// <summary>
    /// 指定した魔法の削除
    /// </summary>
    /// <param name="magicType"></param>
    public void CancelEnemyMagic(eMagicType magicType) {
        if(IsEmpty(magicTypeList) || magicType == eMagicType.Invalid) return;
        for (int i = 0, max = magicTypeList.Count; i < max; i++) {
            eMagicType magic = magicTypeList[i];
            if(magic != magicType) continue;

            MagicReset(eSideType.EnemySide, magicType);
            magicTypeList.Remove(magic);
        }
    }
    /// <summary>
    /// 敵の全ての魔法をリセット
    /// </summary>
    public void CancelAllEnemyMagic() {
        if(IsEmpty(magicTypeList)) return;
        for (int i = magicTypeList.Count - 1; i >= 0; i--) {
            MagicReset(eSideType.EnemySide, magicTypeList[i]);
            magicTypeList.Remove(magicTypeList[i]);
        }
    }
    /// <summary>
    /// 行動開始
    /// </summary>
    public void StartEnemyAction() {
        ChangeAction(eEnemyActionType.Wait);
    }
    /// <summary>
    /// 行動IDデータの設定
    /// </summary>
    /// <param name="masterActionList"></param>
    public void SetActionID(int[] masterActionList) {
        int masterActionCount = masterActionList.Length;
        actionList = new List<int>(masterActionCount);
        // データから-1を除いたアクションIDを設定する
        for (int i = 0; i < masterActionCount; i++) {
            if (masterActionList[i] < 0) continue;

            actionList.Add(masterActionList[i]);
        }
    }
    /// <summary>
    /// クールタイムの取得
    /// </summary>
    /// <returns></returns>
    public float GetCoolTime() {
        return coolTime;
    }
    /// <summary>
    /// クールタイムの設定
    /// </summary>
    public void SetCoolTime() {
        coolTime = Random.Range(decisionData.minCoolTime, decisionData.maxCoolTime);
    }
    /// <summary>
    /// 行動の変更
    /// </summary>
    /// <param name="setAction"></param>
    public void ChangeAction(eEnemyActionType setAction) {
        if(!FindActionID((int)setAction)) return;
        // 現在の行動の片付け処理
        if (currentAction != null) {
            currentAction.Teardown(this);
            prevAction = currentAction;
        }
        // 行動の生成
        currentAction = EnemyActionFactory.Create(setAction);
        if(currentAction == null) return;
        // 更新処理
        currentAction.Setup(this);
        actionType = setAction;
    }
    /// <summary>
    /// 該当するアクションIDが存在するか探す
    /// </summary>
    /// <param name="actionID"></param>
    /// <returns></returns>
    public bool FindActionID(int actionID) {
        for (int i = 0, max = actionList.Count; i < max; i++) {
            if(actionList[i] != actionID) continue;

            return true;
        }
        return false;
    }
    /// <summary>
    /// 行動実施中か判定
    /// </summary>
    /// <returns></returns>
    public bool IsAction() {
        return currentAction != null && !currentAction.IsFinished();
    }
    /// <summary>
    /// アニメーション終了処理の呼び出し
    /// </summary>
    public void EndAnimation() {
        // 変換を試みて、行けたら実行
        if (currentAction is IEnemyEndAnimation end) end.EndAnimation();
    }
}
