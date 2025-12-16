/// <summary>
/// 行動実行インターフェース
/// </summary>
public interface IEnemyAction {
    /// <summary>
    /// アクション準備前処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Setup(EnemyCharacter enemy);
    /// <summary>
    /// アクション実行処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Execute(EnemyCharacter enemy);
    /// <summary>
    /// アクション片付け処理
    /// </summary>
    /// <param name="enemy"></param>
    public void Teardown(EnemyCharacter enemy);
}