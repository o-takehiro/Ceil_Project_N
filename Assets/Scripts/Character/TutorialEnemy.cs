using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TutorialEnemy : EnemyCharacter {
    public static CharacterAIMachine<TutorialEnemy> _myAI { get; private set; } = null;

    public override void Setup() {
        base.Setup();
        _myAI = new CharacterAIMachine<TutorialEnemy>();
        _myAI.Setup(this);
        _myAI.ChangeState(new MoveRight());
    }
    private void Update() {
        _myAI.Update();
    }
    private class MoveRight : CharacterAIBase<TutorialEnemy> {
        public override void Setup() {
            base.Setup();
            ownerClass.gameObject.transform.position = Vector3.zero;
        }
        public override void Execute() {
            base.Execute();
            ownerClass.gameObject.transform.position += Vector3.right;
            if (ownerClass.gameObject.transform.position.x >= 20) {

                _myAI.ChangeState(new MoveLeft());
            }
        }
        public override void Teardown() {
            base.Teardown();
        }
    }

    private class MoveLeft : CharacterAIBase<TutorialEnemy> {
        public override void Setup() {
            base.Setup();
            ownerClass.gameObject.transform.position = Vector3.zero;
        }
        public override void Execute() {
            base.Execute();
            ownerClass.gameObject.transform.position += Vector3.left;
            if (ownerClass.gameObject.transform.position.x <= -20) {
                _myAI.ChangeState(new MoveRight());
            }
        }
        public override void Teardown() {
            base.Teardown();
        }
    }
}
