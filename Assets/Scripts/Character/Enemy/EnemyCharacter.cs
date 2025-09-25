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

    public override void Initialize() {
        base.Initialize();
    }
    public override void Setup(int masterID) {
        base.Setup(masterID);
        var masterData = GetCharacterMaster(masterID);
        SetMaxHP(masterData.HP);
        SetHP(masterData.HP);
        SetRawAttack(masterData.Attack);
        SetRawDefense(masterData.Defense);
        SetMinActionTime(masterData.MinActionTime);
        SetMaxActionTime(masterData.MaxActionTime);
        //現在の位置更新
        SetEnemyPosition(Vector3.zero);
        //一フレーム前の位置更新
        SetEnemyPrevPosition();
        //中心座標の更新
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
        //現在の回転更新
        SetEnemyRotation(Quaternion.identity);
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
    public override void Dead() {
        enemyHPGauge.gameObject.SetActive(false);
        myAI.ChangeState(new EnemyAI008_Empty());
        enemyAnimator.SetBool("isDead", true);
        CancelAllEnemyMagic();
        SetAllActiveCollider(false);
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
        if(IsEmpty(magicTypeList)) return;
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

    public void StartEnemyState() {
        myAI.ChangeState(new EnemyAI001_Wait());
    }
}
