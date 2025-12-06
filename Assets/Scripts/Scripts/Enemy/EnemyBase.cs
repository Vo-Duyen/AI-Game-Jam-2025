using System;
using System.Collections;
using System.Collections.Generic;
using LongNC;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyBase<TState, TEnemyType> : SerializedMonoBehaviour, IEnemy
where TState : Enum
where TEnemyType : Enum
{
    [OdinSerialize] protected TEnemyType _type;
    [OdinSerialize] protected TState _state;
    
    [OdinSerialize] protected Rigidbody2D _rb;
    [OdinSerialize] protected EnemyData _enemyData;

    [OdinSerialize] protected Vector3 _savePoint = Vector3.left * 999;
    protected RaycastHit2D[] _hits = new RaycastHit2D[10];
    protected Collider2D[] _cols = new Collider2D[10];

    protected Vector3[] _directions = new Vector3[]
    {
        Vector3.left, Vector3.right, new Vector3(-1, 1, 0), new Vector3(1, 1, 0)
    };
    
    [Button]
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
    public virtual void Move(Vector3 direction)
    {
        var limit = 0.4f;
        if (Vector3.Distance(direction, _directions[0]) <= limit)
        {
            transform.position += Vector3.left * _enemyData.moveSpeed * Time.deltaTime;
        }
        else if (Vector3.Distance(direction, _directions[1]) <= limit)
        {
            transform.position += Vector3.right * _enemyData.moveSpeed * Time.deltaTime;
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
    public virtual void Patrol(Vector3 targetPoint)
    {
        if (Vector3.Distance(transform.position, targetPoint) <= 0.1f)
        {
            targetPoint.x = Random.Range(_savePoint.x - _enemyData.patrolRange, _savePoint.x + _enemyData.patrolRange);
            Debug.LogWarning(targetPoint);
        }
        var direction = (targetPoint - transform.position).normalized;
        Move(direction);
    }

    [Button]
    public void Follow(Vector3 targetPoint)
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
        var size = Physics2D.OverlapCircleNonAlloc(transform.position, _enemyData.attackFollow, _cols);
        if (size == 0) return false;
        for (var i = 0; i < size; ++i)
        {
            var target = _cols[i].gameObject;
            if (target.TryGetComponent<IPlayer>(out var player))
            {
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
        
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + _enemyData.distanceCheckGround * Vector3.down);
        
        // patrol range
        if (!Mathf.Approximately(_savePoint.x, -999f))
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(_savePoint + Vector3.left * _enemyData.patrolRange, _savePoint + Vector3.right * _enemyData.patrolRange);
        }
        
        
    }
}
