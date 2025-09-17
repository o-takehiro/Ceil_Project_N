using UnityEngine;

using static CharacterUtility;

public class MageAnimationEvents : MonoBehaviour {
    [SerializeField] private Collider attackCollider;   // コライダー
    private PlayerAttack _playerAttack;                 // PlayerAttackクラス
    private PlayerMovement _playerMovement;             // PlayerMovementクラス
    public static bool isGameOver = false;
    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start() {
        if (attackCollider != null) attackCollider.enabled = false;
        // プレイヤーを取得
        var player = CharacterUtility.GetPlayer();
        // PlayerAttackクラスを取得
        if (player != null) _playerAttack = player.GetAttackController();
        // PlayerMovementクラスを取得
        if (player != null) _playerMovement = player.GetPlayerMovement();
        isGameOver = false;
    }

    // コライダー制御
    public void OnCollider() => attackCollider.enabled = true;
    public void OffCollider() => attackCollider.enabled = false;

    // 攻撃毎のサウンド
    public void AttackPlaySE_1() => SoundManager.Instance.PlaySE(0);
    public void AttackPlaySE_2() => SoundManager.Instance.PlaySE(1);
    public void AttackPlaySE_3() => SoundManager.Instance.PlaySE(2);

    // アニメーション内のイベント
    public void OnComboCheck() {
        _playerAttack?.TryNextCombo(); // 次段移行判定
    }
    public void OnAttackEnd() {
        _playerAttack?.EndAttack();
    }

    public void IsSetAttack() {
        _playerAttack._isAttacking = true;
    }

    public void IsNotSetAttack() {
        _playerAttack._isAttacking = false;
    }

    /// <summary>
    /// 三段目入力拒否
    /// </summary>
    public void DisableAttackInput() => _playerAttack?.SetCanReceiveInput(false);
    public void EnableAttackInput() => _playerAttack?.SetCanReceiveInput(true);


    /// <summary>
    /// 敵へのダメージ処理
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Enemy")) return;
        int damage = _playerAttack?.GetDamageValue() ?? 0;
        _playerAttack.AddMP();
        ToEnemyDamage(damage);
    }

    // アニメーションイベントから呼ばれる
    public void DisableJump() {
        _playerMovement._canJump = false;
    }
    public void EnableJump() {
        _playerMovement._canJump = true;
    }

    public void UnusePlayerFlag() {
        UnusePlayer();
        isGameOver = true;
    }
}
