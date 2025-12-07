using System;
using DesignPattern.ObjectPool;
using Sirenix.Serialization;
using UnityEngine;

public class EnemyController : EnemyBase<EnemyController.State, EnemyController.EnemyType>
{
    public enum EnemyType
    {
        A,
        B
    }
    public enum State
    {
        Idle,
        Patrol,
        Follow,
        Attack,
        GetHit,
        Die,
    }
    
    private bool _isDelayStart = true;

    protected override void Awake()
    {
        base.Awake();
        OnDelayCall(1f, () =>
        {
            _isDelayStart = false;
        });
    }

    protected override void OnPlayerHit(object param)
    {
        if (param is (IEnemy enemy, float damage))
        {
            if (enemy == (IEnemy)this)
            {
                GetHit();
                _currentHealth -= damage;
                if (_currentHealth <= 0)
                {
                    _currentHealth = 0;
                    ChangeState(State.Die);
                }
            }
        }
    }

    public override void ChangeState<T>(T state)
    {
        base.ChangeState(state);
        switch (_state)
        {
            case State.Idle:
                break;
            case State.Patrol:
                SavePoint();
                _targetPoint = transform.position;
                break;
            case State.Follow:
                break;
            case State.Attack:
                _targetPoint.x = _player.GetTransform().position.x;
                Attack();
                break;
            case State.GetHit:
                GetHit();
                break;
            case State.Die:
                Die();
                break;
        }
    }

    private void Update()
    {
        if (_isDelayStart) return;
        if (_enemyData == null) return;
        switch (_state)
        {
            case State.Idle:
                if (IsHavePlayer())
                {
                    ChangeState(State.Follow);
                }
                else if (IsGrounded())
                {
                    ChangeState(State.Patrol);
                }
                break;
            case State.Patrol:
                Patrol();
                if (IsHavePlayer())
                {
                    ChangeState(State.Follow);
                }
                break;
            case State.Follow:
                Follow();
                break;
            case State.Attack:
                break;
        }
    }

    public override void Follow()
    {
        var distanceTarget = Vector3.Distance(transform.position, _targetPoint);
        if (distanceTarget <= _enemyData.attackRange)
        {
            ChangeState(State.Attack);
            return;
        }

        if (distanceTarget >= _enemyData.attackFollowRange)
        {
            ChangeState(State.Patrol);
            return;
        }
        var direction = (_targetPoint - transform.position).normalized;
        Move(direction, _enemyData.runSpeeed);
    }

    public override void GetHit()
    {
        _animator.Play(_animGetHit.name);
        OnDelayCall(_animGetHit.length + 0.1f, () =>
        {
            ChangeState(State.Follow);
        });
    }

    public override void Attack()
    {
        RotateWithTarget(_targetPoint);
        _animator.Play(_animAttack.name);
        
        OnDelayCall(_animAttack.length / 2f, () =>
        {
            HitPlayer();
        });
        
        OnDelayCall(_animAttack.length + 0.1f, () =>
        {
            ChangeState(State.Follow);
        });
    }
}
