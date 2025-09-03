using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーの攻撃処理（コンボ管理、アニメーション再生など）
/// </summary>
public class PlayerAttack {
    private readonly Rigidbody _rigidbody;   // Rigidbody参照
    private readonly Animator _animator;     // Animator参照

    private Dictionary<AttackStep, AttackData> _attackDataMap; // 攻撃ごとの設定
    private AttackStep _currentAttack = AttackStep.Invalid;    // 現在の攻撃段階

    private bool _attackRequested;   // 攻撃要求フラグ
    private bool _isAttacking;       // 攻撃中フラグ
    private float _attackTimer;      // 攻撃リセット用のタイマー

    private const float ATTACK_RESET_TIME = 5f; // コンボリセットまでの秒数

    // 攻撃段階
    private enum AttackStep {
        Invalid,
        First,
        Second,
        Third
    }

    // 攻撃中かどうか外部から参照できる
    public bool IsAttacking => _isAttacking;

    public PlayerAttack(Rigidbody rigidbody, Animator animator) {
        _rigidbody = rigidbody;
        _animator = animator;
    }

    /// <summary>
    /// 攻撃要求を受け付ける
    /// </summary>
    public void RequestAttack() {
        if (!_isAttacking) _attackRequested = true;
    }

    /// <summary>
    /// 攻撃データ（アニメーション名や硬直時間など）の初期化
    /// </summary>
    public void SetupAttackData() {
        _attackDataMap = new Dictionary<AttackStep, AttackData> {
            { AttackStep.First, new AttackData("attack", 10f, 500, 0) },
            { AttackStep.Second, new AttackData("attack", 15f, 500, 0) },
            { AttackStep.Third, new AttackData("attack", 20f, 1000, 3) }
        };
    }

    /// <summary>
    /// 1フレーム分の攻撃処理（非同期）
    /// </summary>
    public async UniTask Update(float deltaTime) {

        // 攻撃要求があった場合の処理
        if (_attackRequested && !_isAttacking) {
            _attackRequested = false;
            _isAttacking = true;

            // 攻撃開始時に移動を止める
            _rigidbody.velocity = Vector3.zero;

            // 次の攻撃段階に進める
            AdvanceAttackStep();

            // 攻撃データを取得
            if (!_attackDataMap.TryGetValue(_currentAttack, out var attackData)) {
                _isAttacking = false;
                return;
            }
            if (_animator != null)
                _animator.SetTrigger(attackData.AnimationName);
            // アニメーション再生
            _animator.SetTrigger(attackData.AnimationName);

            // コライダー有効時間待機
            await UniTask.Delay(attackData.ColliderActiveDurationMs);

            // 硬直時間待機
            await UniTask.Delay(attackData.PostDelayMs);

            _isAttacking = false;
        }

        // コンボリセット判定
        if (_currentAttack != AttackStep.Invalid) {
            _attackTimer += deltaTime;
            if (_attackTimer >= ATTACK_RESET_TIME) {
                _currentAttack = AttackStep.Invalid;
                _attackTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 現在の攻撃段階を次に進める
    /// </summary>
    private void AdvanceAttackStep() {
        _currentAttack = _currentAttack switch {
            AttackStep.Invalid => AttackStep.First,
            AttackStep.First => AttackStep.Second,
            AttackStep.Second => AttackStep.Third,
            _ => AttackStep.First
        };
    }
}
