using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static CharacterUtility;

public class EnemyCommonModule {
    public static void LookAtPlayer(float setTime = 0.1f) {
        //ƒvƒŒƒCƒ„[‚Ì•ûŒü‚ğŒü‚«‘±‚¯‚é
        Quaternion enemyRotation = GetEnemyRotation();
        //•ûŒü‚ğŒˆ‚ß‚é
        Vector3 direction = GetPlayerPosition() - GetEnemyPosition();
        //…•½•ûŒü‚Ì‚İ‚Ì‰ñ“]‚Ì‚½‚ßAy‚É‚Í0‚ğ‘ã“ü
        direction.y = 0;
        if(direction == Vector3.zero) return; 

        //‰ñ“]‚³‚¹‚é
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        enemyRotation = Quaternion.Slerp(enemyRotation, lookRotation, setTime);
        //©g‚Ì‰ñ“]‚É‘ã“ü‚·‚é
        SetEnemyRotation(enemyRotation);
    }

    public static void LookAtDirection(Vector3 setDirection) {
        //•ûŒü‚ğŒˆ‚ß‚é
        Vector3 direction = setDirection;
        if (direction == Vector3.zero) return;

        //“Á’è‚Ì•ûŒü‚ğŒü‚«‘±‚¯‚é
        Quaternion enemyRotation = GetEnemy().transform.rotation;
        //…•½•ûŒü‚Ì‚İ‚Ì‰ñ“]‚Ì‚½‚ßAy‚É‚Í0‚ğ‘ã“ü
        direction.y = 0;
        //‰ñ“]‚³‚¹‚é
        Quaternion lookRotation = Quaternion.LookRotation(direction, Vector3.up);
        enemyRotation = Quaternion.Slerp(enemyRotation, lookRotation, 0.1f);
        //©g‚Ì‰ñ“]‚É‘ã“ü‚·‚é
        SetEnemyRotation(enemyRotation);
    }
}
