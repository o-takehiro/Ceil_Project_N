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