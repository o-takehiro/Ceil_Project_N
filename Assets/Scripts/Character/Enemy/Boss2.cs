using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;
using static CharacterMasterUtility;

public class Boss2 : EnemyCharacter {
    public override void Initialize() {
        base.Initialize();
    }

    public override void Setup(int masterID) {
        base.Setup(masterID);
        var characterMaster = GetCharacterMaster(masterID);
        SetMaxHP(characterMaster.HP);
        SetHP(characterMaster.HP);
        SetRawAttack(characterMaster.Attack);
        SetRawDefense(characterMaster.Defense);
        //現在の位置更新
        SetEnemyPosition(transform.position);
        //一フレーム前の位置更新
        SetEnemyPrevPosition();
        //中心座標更新
        SetEnemyCenterPosition(new Vector3(transform.position.x, transform.position.y + 2, transform.position.z));
        //現在の回転更新
        SetEnemyRotation(transform.rotation);
        //HPゲージの更新
        SetupCanvasPosition(Vector3.one * 3);
    }

    public override void Teardown() {
        base.Teardown();
    }
}
