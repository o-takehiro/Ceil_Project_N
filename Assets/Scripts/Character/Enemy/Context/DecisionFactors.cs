/// <summary>
/// 敵の行動判断材料構造体
/// </summary>
public struct DecisionFactors {
    public bool isCoolTime;             // 攻撃後のクールタイム判定フラグ
    public bool isPlayerClose;          // プレイヤーが一定距離以下か判定フラグ
    public bool isPlayerFar;            // プレイヤーが一定距離移動か判定フラグ
    public bool[] isPlayerActiveMagic;  // プレイヤーが特定の魔法を使用しているか判定フラグ
}
