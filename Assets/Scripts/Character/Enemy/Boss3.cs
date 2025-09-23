using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameConst;
using static CharacterUtility;

public class Boss3 : EnemyCharacter {
    public override void Initialize() {
        base.Initialize();
        actionMachine = new EnemyAI011_Boss3Action();
        myAI = new CharacterAIMachine<EnemyCharacter>();
        enemyAnimator = GetComponent<Animator>();
        magicTypeList = new List<eMagicType>(MAX_ENEMY_MAGIC);
    }

    public override void Setup(int masterID) {
        base.Setup(masterID);
        //中心座標更新
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 20, transform.position.z));
        myAI.Setup(this);
        myAI.ChangeState(new EnemyAI001_Wait());
        SetupCanvasPosition(new Vector3(0, -250, 0), new Vector2(400, 40));
    }

    protected void SetupCanvasPosition(Vector3 setPosition, Vector2 setDelta) {
        SetEnemyCanvas();
        RectTransform rectGauge = enemyHPGauge.GetComponent<RectTransform>();
        enemyHPGauge.transform.SetParent(enemyCanvas.transform, false);
        enemyHPGauge.transform.localPosition = setPosition;
        enemyHPGauge.transform.localRotation = Quaternion.Euler(0, 0, 0); // 毎回180度に確定
        rectGauge.sizeDelta = setDelta;
        enemyCanvas.gameObject.SetActive(true);
        enemyHPGauge.gameObject.SetActive(true);
    }
    private void Update() {
        //現在の位置更新
        SetEnemyPosition(transform.position);
        //現在の回転更新
        SetEnemyRotation(transform.rotation);
        //中心座標更新
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 20, transform.position.z));
        //ステートマシーンの更新
        myAI.Update();
        //座標の更新
        transform.position = GetEnemyPosition();
        transform.rotation = GetEnemyRotation();
        //一フレーム前の位置更新
        SetEnemyPrevPosition();
    }
    public override void Teardown() {
        base.Teardown();
    }
}
