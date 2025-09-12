using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack {
    private readonly Rigidbody _rigidbody;      // RigitBody
    private readonly Animator _animator;        // Animator

    private Dictionary<AttackStep, AttackData> _attackDataMap;
    private AttackStep _currentAttack = AttackStep.Invalid;

    private bool _attackRequested;  // 次の攻撃要求
    public bool _isAttacking;      // 現在攻撃中
    private float _attackTimer;     // 攻撃の遷移時間

    private const float ATTACK_RESET_TIME = 5f;     // 攻撃がリセットされる時間
    private int _playerRawAttack;                   // プレイヤーの基礎攻撃力
    private bool _canReceiveInput = true;  // 入力受付フラグ
    private enum AttackStep {
        Invalid,
        First,
        Second,
        Third
    }

    public bool IsAttacking => _isAttacking;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="rigidbody"></param>
    /// <param name="animator"></param>
    /// <param name="rawAttack"></param>
    public PlayerAttack(Rigidbody rigidbody, Animator animator, int rawAttack) {
        _rigidbody = rigidbody;
        _animator = animator;
        _playerRawAttack = rawAttack;
    }

    /// <summary>
    /// 入力を受け付ける
    /// </summary>
    public void RequestAttack() {
        if (!_canReceiveInput) return;  // 入力禁止中なら無視


        _attackRequested = true;

        // 初撃を出す
        if (!_isAttacking) {
            StartAttack();
        }

    }

    /// <summary>
    /// AttackDataに攻撃の情報を渡す
    /// </summary>
    public void SetupAttackData() {
        _attackDataMap = new Dictionary<AttackStep, AttackData> {
            { AttackStep.First, new AttackData("attack", 1.3f, 500, 0) },
            { AttackStep.Second, new AttackData("attack", 1.5f, 500, 0) },
            { AttackStep.Third, new AttackData("attack", 2.0f, 1000, 3) }
        };
    }

    /// <summary>
    /// 更新処理
    /// </summary>
    /// <param name="deltaTime"></param>
    public void AttackUpdate(float deltaTime) {
        // コンボリセット管理
        if (!_isAttacking && _currentAttack != AttackStep.Invalid) {
            _attackTimer += deltaTime;
            if (_attackTimer >= ATTACK_RESET_TIME) {
                _currentAttack = AttackStep.Invalid;
                _attackTimer = 0f;
            }
        }
    }

    /// <summary>
    /// 攻撃開始処理
    /// </summary>
    private void StartAttack() {
        _isAttacking = true;
        _attackRequested = false;

        // 次の攻撃ステップへ進む
        AdvanceAttackStep();

        // アニメーションを再生
        if (_attackDataMap.TryGetValue(_currentAttack, out var attackData)) {
            _animator?.SetTrigger(attackData.AnimationName);
            _rigidbody.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// アニメーション遷移用
    /// </summary>
    public void TryNextCombo() {
        if (_attackRequested) {
            StartAttack();
        }
    }
    /// <summary>
    /// アニメーション終了時に呼ぶ
    /// </summary>
    public void EndAttack() {
        _isAttacking = false;
        _attackRequested = false;
    }

    /// <summary>
    /// 攻撃遷移
    /// </summary>
    private void AdvanceAttackStep() {
        _currentAttack = _currentAttack switch {
            AttackStep.Invalid => AttackStep.First,
            AttackStep.First => AttackStep.Second,
            AttackStep.Second => AttackStep.Third,
            _ => AttackStep.First
        };
        _attackTimer = 0f;
    }

    /// <summary>
    /// プレイーの与えるダメージを受け取る
    /// </summary>
    /// <returns></returns>
    public int GetDamageValue() {
        if (_attackDataMap.TryGetValue(_currentAttack, out var data)) {
            return Mathf.RoundToInt(_playerRawAttack * data.Damage);
        }
        return _playerRawAttack;
    }

    /// <summary>
    /// 三段目に派生しないように
    /// </summary>
    /// <param name="value"></param>
    public void SetCanReceiveInput(bool value) {
        _canReceiveInput = value;
    }

    /// <summary>
    /// 攻撃関連をリセットする
    /// </summary>
    public void ResetState() {
        _attackRequested = false;
        _isAttacking = false;
        _currentAttack = AttackStep.Invalid;
        _attackTimer = 0f;
    }
}
