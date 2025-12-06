using System;
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
    }

    private Vector3 _targetPoint;
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
                Patrol(_savePoint);
                break;
            case State.Follow:
                break;
            case State.Attack:
                break;
        }
    }
    
}
