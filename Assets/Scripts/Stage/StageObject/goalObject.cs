using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class goalObject : StageObjectBase {

    public override void SetUp() {
        base.SetUp();

    }

    protected override void OnUpdate() {
    }

    private void OnTriggerEnter(Collider collision) {
        if(collision.gameObject.layer == 6) {
            // SEçƒê∂
                

        }
    }

}
