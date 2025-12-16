/// <summary>
/// 行動判断インターフェース
/// </summary>
public interface IEnemyDecision {
    /// <summary>
    /// 行動判断処理
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public eEnemyActionType Decide(EnemyFactors foctors);
}
