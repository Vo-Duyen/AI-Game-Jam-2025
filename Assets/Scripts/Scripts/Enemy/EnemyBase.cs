using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

public class EnemyBase<TState, TEnemyType> : SerializedMonoBehaviour, IEnemy
where TState : Enum
where TEnemyType : Enum
{
    [OdinSerialize] protected TEnemyType _type;
    [OdinSerialize] protected TState _state;

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
    }

    [Button]
    public virtual void Reverse()
    {
        
    }

    [Button]
    public virtual void Jump()
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
}
