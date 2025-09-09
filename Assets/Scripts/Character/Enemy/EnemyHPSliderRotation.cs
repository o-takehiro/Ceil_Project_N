using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class EnemyHPSliderRotation : MonoBehaviour {
    private void Update() {
        LookatPlayer();  
    }

    private void LookatPlayer() {
        // ƒJƒƒ‰‚Ö‚Ì•ûŒü
        Vector3 direction = Camera.main.transform.position - transform.position;

        // ‚‚³‚ğ–³‹‚µ‚Ä…•½‰ñ“]‚¾‚¯‚É‚·‚é
        direction.y = 0;

        if (direction.sqrMagnitude < 0.001f)
            return; // ƒ[ƒœ‚¯

        // ‚»‚Ì‚Ü‚Ü‰ñ“]‚ğ“K—pi•âŠÔ‚È‚µAu‚É‰ñ“]j
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }
}
