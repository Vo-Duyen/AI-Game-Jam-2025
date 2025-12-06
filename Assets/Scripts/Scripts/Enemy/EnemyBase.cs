using System;
using System.Collections;
using System.Collections.Generic;
using DesignPattern.ObjectPool;
using LongNC;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBase<TState, TEnemyType> : SerializedMonoBehaviour, IEnemy
where TState : Enum
where TEnemyType : Enum
{
    protected const string SetupString = "Setup";
    protected const string AddComponentString = SetupString + "/AddComponent";
    protected const string AnimString = SetupString + "/Anim";
    
    [BoxGroup(SetupString), OdinSerialize] protected TEnemyType _type;
    [BoxGroup(SetupString), OdinSerialize] protected TState _state;
    
    [FoldoutGroup(AddComponentString), OdinSerialize] protected Rigidbody2D _rb;
    [FoldoutGroup(AddComponentString), OdinSerialize] protected Animation _animation;
    [FoldoutGroup(AddComponentString), OdinSerialize] protected EnemyData _enemyData;
    
    [FoldoutGroup(AnimString), OdinSerialize] protected AnimationClip _animMove;
    [FoldoutGroup(AnimString), OdinSerialize] protected AnimationClip _animRun;
    [FoldoutGroup(AnimString), OdinSerialize] protected AnimationClip _animJump;
    [FoldoutGroup(AnimString), OdinSerialize] protected AnimationClip _animAttack;
    [FoldoutGroup(AnimString), OdinSerialize] protected AnimationClip _animGetHit;
    [FoldoutGroup(AnimString), OdinSerialize] protected AnimationClip _animDeath;

    protected Vector3 _savePoint = Vector3.left * 999;
    protected RaycastHit2D[] _hits = new RaycastHit2D[10];
    protected Collider2D[] _cols = new Collider2D[10];
    protected Vector3 _targetPoint;
    protected IPlayer _player;

    protected Vector3[] _directions = new Vector3[]
    {
        Vector3.left, Vector3.right, new Vector3(-1, 1, 0), new Vector3(1, 1, 0)
    };
    
    [FoldoutGroup(AddComponentString), Button]
    protected virtual void GetRigidbody()
    {
        if (transform.TryGetComponent<Rigidbody2D>(out var rb))
        {
            _rb = rb;
            return;
        }
        Queue<Transform> queue = new Queue<Transform>();
        queue.Enqueue(transform);
        while (queue.Count > 0)
        {
            var target = queue.Dequeue();
            if (target.TryGetComponent<Rigidbody2D>(out var rbChild))
            {
                _rb = rbChild;
                return;
            }

            foreach (Transform child in target)
            {
                queue.Enqueue(child);
            }
        }
    }

    [FoldoutGroup(AddComponentString), Button]
    protected virtual void AddAnimation()
    {
        if (!transform.TryGetComponent<Animation>(out var animation))
        {
            animation = gameObject.AddComponent<Animation>();
        }
        _animation = animation;
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public virtual T GetType<T>() where T : Enum
    {
        if (typeof(T) == typeof(TEnemyType) && _type is T result)
        {
            return result;
        }

        throw new InvalidOperationException(
            $"Cannot retrieve type. Requested type {typeof(T).Name} does not match " +
            $"the character's base type {typeof(TEnemyType).Name}."
        );
    }

    public virtual bool IsType<T>(T type) where T : Enum
    {
        if (type is TEnemyType t) return _type.Equals(t);
        return false;
    }

    public virtual void ChangeType<T>(T type)  where T : Enum
    {
        if (type is TEnemyType t) 
        {
            _type = t;
        }
        Debug.LogWarning($"{type} not type {_type}");
    }
    
    public virtual bool IsState<T>(T state) where T : Enum
    {
        if (state is TState t) return _state.Equals(t);
        return false;
    }

    public virtual void ChangeState<T>(T state) where T : Enum
    {
        if (typeof(T) != typeof(TState)) return;
        if (state is TState s)
        {
            _state = s;
        }
    }
    
    [Button]
    public virtual void Reverse()
    {
        
    }

    [Button]
    public virtual void Jump()
    {
        if (_rb == null) return;
        _rb.AddForce(new Vector2(0f, _enemyData.jumpForce));
    }

    [Button]
    public virtual void Move(Vector3 direction, float speed)
    {
        var limit = 0.4f;
        if (Vector3.Distance(direction, _directions[0]) <= limit)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        else if (Vector3.Distance(direction, _directions[1]) <= limit)
        {
            transform.position += Vector3.right * speed * Time.deltaTime;
        }
        else if (Vector3.Distance(direction, _directions[2]) <= limit)
        {
            Jump();
        }
        else if (Vector3.Distance(direction, _directions[3]) <= limit)
        {
            Jump();
        }
        else
        {
            Debug.LogWarning($"Direction {direction} not supported");
        }
    }
    
    [Button]
    public virtual void Patrol()
    {
        if (Vector3.Distance(transform.position, _targetPoint) <= 0.1f)
        {
            var newX = Random.Range(_savePoint.x - _enemyData.patrolRange, _savePoint.x + _enemyData.patrolRange);
            while (Mathf.Abs(newX - _targetPoint.x) <= _enemyData.patrolRange / 2f)
            {
                newX = Random.Range(_savePoint.x - _enemyData.patrolRange, _savePoint.x + _enemyData.patrolRange);
            }
            _targetPoint.x = newX;
        }
        var direction = (_targetPoint - transform.position).normalized;
        Move(direction, _enemyData.moveSpeed);
    }

    [Button]
    public virtual void Follow()
    {
    }

    [Button]
    public virtual void Run()
    {
        
    }
    
    [Button]
    public virtual void Attack()
    {
        
    }

    [Button]
    public virtual void GetHit()
    {
        
    }

    [Button]
    public virtual void Die()
    {
        _animation.Play(_animAttack.name);
        OnDelayCall(_animAttack.length + 0.1f, () =>
        {
            PoolingManager.Despawn(gameObject);
        });
    }

    protected virtual bool IsGrounded()
    {
        if (_enemyData == null) return false;
        var size = Physics2D.RaycastNonAlloc(transform.position, Vector2.down, _hits, _enemyData.distanceCheckGround);
        if (size <= 1) return false;
        // TODO: Fix late
        return true;
    }

    protected virtual bool IsHavePlayer()
    {
        if (_enemyData == null) return false;
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, _enemyData.attackFollowRange, _cols);
        if (size == 0) return false;
        for (var i = 0; i < size; ++i)
        {
            var target = _cols[i].gameObject;
            if (target.TryGetComponent<IPlayer>(out var player))
            {
                _player = player;
                return true;
            }
        }
        return false;
    }

    public virtual void SavePoint(float timeDelay = 0f)
    {
        if (timeDelay <= 0f)
        {
            _savePoint = transform.position;
            return;
        }
        
        OnDelayCall(timeDelay, () =>
        {
            _savePoint = transform.position;
        });
    }

    protected virtual void OnDelayCall(float timeDelay, Action callback)
    {
        StartCoroutine(IEDelayCall(timeDelay, callback));
    }
    
    protected virtual IEnumerator IEDelayCall(float timeDelay, Action callback)
    {
        yield return WaitForSecondCache.Get(timeDelay);
        callback?.Invoke();
    }

    protected void OnDrawGizmos()
    {
        if (_enemyData == null) return;
        
        // Check ground
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + _enemyData.distanceCheckGround * Vector3.down);
        
        // patrol range
        if (!Mathf.Approximately(_savePoint.x, -999f))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_savePoint + Vector3.left * _enemyData.patrolRange, _savePoint + Vector3.right * _enemyData.patrolRange);
        }
        
        Gizmos.color = Color.yellow;
        DrawCircle(transform.position, _enemyData.attackFollowRange, 60);
    }
    protected virtual void DrawCircle(Vector3 center, float radius, int segments)
    {
        var angleStep = 360f / segments;
        var prevPoint = center + new Vector3(radius, 0, 0);

        for (var i = 1; i <= segments; i++)
        {
            var angle = i * angleStep * Mathf.Deg2Rad;
            var newPoint = center + new Vector3(
                Mathf.Cos(angle) * radius,
                Mathf.Sin(angle) * radius,
                0
            );
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}
