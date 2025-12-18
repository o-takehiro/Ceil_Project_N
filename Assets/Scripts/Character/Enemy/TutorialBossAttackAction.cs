using UnityEngine;

using static CharacterUtility;
using static MagicUtility;

public class TutorialBossAttackAction : MonoBehaviour {
    /// <summary>
    /// UŒ‚–‚–@”­“®
    /// </summary>
    public void EnableAttackMagic() {
        CreateMagic(eSideType.EnemySide, eMagicType.MiniBullet);
    }
    /// <summary>
    /// UŒ‚–‚–@‰ğœ
    /// </summary>
    public void DisableAttackMagic() {
        MagicReset(eSideType.EnemySide, eMagicType.MiniBullet);
    }
}
