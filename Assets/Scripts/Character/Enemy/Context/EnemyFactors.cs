/// <summary>
/// 敵の行動判断材料構造体
/// </summary>
public struct EnemyFactors {
    public float distanceToPlayer;      // プレイヤーとの距離
    public float closePlayerDistance;   // 許容近距離
    public float farPlayerDistance;     // 許容遠距離
}
