using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack {
    private readonly Rigidbody _rigidbody;
    private readonly Animator _animator;

    private Dictionary<AttackStep, AttackData> _attackDataMap;
    private AttackStep _currentAttack = AttackStep.Invalid;

    private bool _attackRequested;  // 次の攻撃要求
    private bool _isAttacking;      // 現在攻撃中
    private float _attackTimer;

    private const float ATTACK_RESET_TIME = 5f;
    private int _playerRawAttack;
    private bool _canReceiveInput = true;  // 入力受付フラグ
    private enum AttackStep {
        Invalid,
        First,
        Second,
        Third
    }

    public bool IsAttacking => _isAttacking;

    public PlayerAttack(Rigidbody rigidbody, Animator animator, int rawAttack) {
        _rigidbody = rigidbody;
        _animator = animator;
        _playerRawAttack = rawAttack;
    }

    public void RequestAttack() {
        if (!_canReceiveInput) return;  // 入力禁止中なら無視

        _attackRequested = true;

        // 攻撃中じゃないなら、すぐに最初の攻撃を開始
        if (!_isAttacking) {
            StartAttack();
        }

    }

    public void SetupAttackData() {
        _attackDataMap = new Dictionary<AttackStep, AttackData> {
            { AttackStep.First, new AttackData("attack", 1.3f, 500, 0) },
            { AttackStep.Second, new AttackData("attack", 1.5f, 500, 0) },
            { AttackStep.Third, new AttackData("attack", 2.0f, 1000, 3) }
        };
    }

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
    /// 攻撃開始処理（最初の段 or 次段へ移行）
    /// </summary>
    private void StartAttack() {
        _isAttacking = true;
        _attackRequested = false; // 今の入力は消費

        AdvanceAttackStep();


        if (_attackDataMap.TryGetValue(_currentAttack, out var attackData)) {
            _animator?.SetTrigger(attackData.AnimationName);
            _rigidbody.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// 攻撃アニメーション内の「次段移行タイミング」で呼ぶ
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

    private void AdvanceAttackStep() {
        _currentAttack = _currentAttack switch {
            AttackStep.Invalid => AttackStep.First,
            AttackStep.First => AttackStep.Second,
            AttackStep.Second => AttackStep.Third,
            _ => AttackStep.First
        };
        _attackTimer = 0f;
    }

    public int GetDamageValue() {
        if (_attackDataMap.TryGetValue(_currentAttack, out var data)) {
            return Mathf.RoundToInt(_playerRawAttack * data.Damage);
        }
        return _playerRawAttack;
    }

    public void SetCanReceiveInput(bool value) {
        _canReceiveInput = value;
    }

    public void ResetState() {
        _attackRequested = false;
        _isAttacking = false;
        _currentAttack = AttackStep.Invalid;
        _attackTimer = 0f;
    }
}
