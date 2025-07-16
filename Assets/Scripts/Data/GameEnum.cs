/*
 * @file GameEnum.cs
 * @brief 列挙体定義
 */

/// <summary>
/// パート
/// </summary>
public enum eGamePart {
    Invalid = -1,
    Standby,    // 準備パート
    Title,      // タイトルパート
    MainGame,   // メインパート
    Ending,     // エンディングパート
    Max,

}

/// <summary>
/// ゲーム終了要因
/// </summary>
public enum eGameEndReason {
    Invalid = -1,   // 終了していない　
    Dead,           // プレイヤー死亡
    Clear,          // ゲームクリア

}

/// <summary>
/// フェードの画像種類
/// </summary>
public enum FadeType {
    White,  // 白
    Black,  // 黒
    Max,
}

public enum eEnemyType {
    Invalid = -1,
    TutorialEnemy,
    Stage1Enemy,
    Stage2Enemy,
    Stage3Enemy,

    Max
}

/// <summary>
/// 魔法を使う陣営の種類
/// </summary>
public enum eSideType {
    Invalid = -1,
    PlayerSide,     // プレイヤー
    EnemySide,      // 敵

    Max,
}

/// <summary>
/// 魔法の種類
/// </summary>
public enum eMagicType {
    Invalid = -1,
	Defense,        // 防御魔法
	MiniBullet,     // 小型弾幕魔法

	Max,
}
