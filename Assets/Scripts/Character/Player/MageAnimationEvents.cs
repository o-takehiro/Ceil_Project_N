using UnityEngine;

using static CharacterUtility;
public class MageAnimationEvents : MonoBehaviour {
    [SerializeField] private Collider attackCollider;
    //[SerializeField] private Renderer AttackRenderer;
    [SerializeField] private Animator animator;
    // ダメージを与える相手
    private string _TARGET_TAG = "Enemy";
    // プレイヤーの素の攻撃力
    private int _playerRawAttack;
    // プレイヤー参照
    private PlayerCharacter _player;
    private PlayerAttack _playerAttack;
    void Start() {
        //AttackRenderer.enabled = false;
        if (attackCollider != null) {
            attackCollider.enabled = false;
        }

        _player = GetPlayer();
        if (_player != null) {
            _playerRawAttack = _player.GetRawAttack();                 // 素の攻撃力
            _playerAttack = _player.GetAttackController();            // 攻撃コントローラー
        }

    }

    // 攻撃用アニメーションイベント
    public void OnCollider() {
        if (attackCollider != null) {
            attackCollider.enabled = true;
            //AttackRenderer.enabled = true;
        }
    }
    public void OffCollider() {
        if (attackCollider != null) {
            attackCollider.enabled = false;
            //AttackRenderer.enabled = false;
        }
    }

    /// <summary>
    /// ダメージを与える
    /// </summary>
    /// <param name="other"></param>
    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag(_TARGET_TAG)) return;

        // 素の攻撃力
        int baseAttack = _playerRawAttack;

        // 現在の攻撃段階データ取得
        var attackData = _playerAttack?.GetCurrentAttackData();
        float finalDamage = baseAttack;

        if (attackData != null) {
            finalDamage = baseAttack * attackData.Damage;  // 素の攻撃力にコンボ倍率をかける
        }
        Debug.Log((int)finalDamage);
        ToEnemyDamage((int)finalDamage);
    }

}
