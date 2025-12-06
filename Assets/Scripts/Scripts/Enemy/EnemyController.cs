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

    private void Awake()
    {
        OnDelayCall(1f, () =>
        {
            _isDelayStart = false;
        });
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
        if (Vector3.Distance(transform.position, _targetPoint) <= _enemyData.attackRange)
        {
            ChangeState(State.Attack);
            return;
        }
        var direction = (_targetPoint - transform.position).normalized;
        Move(direction, _enemyData.runSpeeed);
    }

    public override void GetHit()
    {
        _animation.Play(_animGetHit.name);
        OnDelayCall(_animGetHit.length + 0.1f, () =>
        {
            ChangeState(State.Follow);
        });
    }

    public override void Attack()
    {
        _animation.Play(_animAttack.name);
        OnDelayCall(_animAttack.length + 0.1f, () =>
        {
            ChangeState(State.Follow);
        });
    }
}
