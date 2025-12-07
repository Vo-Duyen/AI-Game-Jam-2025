using DesignPattern.ObjectPool;
using DesignPattern.Observer;
using System;
using System.Collections;
using UnityEngine;

public class TimePiece : TimeControlled
{
    [Header("Movement Stats")]
    [SerializeField] private float _throwSpeed = 12f;
    [SerializeField] private float _returnSpeed = 18f;
    [SerializeField] private float _maxDistance = 7f;
    
    [Header("Effect Stats")]
    [SerializeField] private float _hangTime = 0.5f;
    [SerializeField] private float _damage = 10f;

    private Vector2 _startPos;
    private Transform _owner;
    protected ObserverManager<GameEvent> observer => ObserverManager<GameEvent>.Instance;

    private bool _isReturning = false;
    private bool _isHanging = false;

    public void Initialize(Transform owner, Vector2 direction)
    {
        _owner = owner;
        _startPos = transform.position;

        direction.Normalize();

        Velocity = direction * _throwSpeed;

        if (TimeController.Instance != null)
        {
            TimeController.Instance.RegisterObject(this);
        }
    }

    public override void TimeUpdate()
    {
        if (_isHanging) return;

        if (!_isReturning)
        {
            // --- GIAI ĐOẠN 1: BAY RA ---
            transform.Translate(Velocity * Time.deltaTime);

            if (Vector2.Distance(_startPos, transform.position) >= _maxDistance)
            {
                StartCoroutine(HangRoutine());
            }
        }
        else
        {
            // --- GIAI ĐOẠN 3: BAY VỀ ---
            if (_owner == null)
            {
                RemovePiece();
                return;
            }

            Vector2 directionToPlayer = (_owner.position - transform.position).normalized;
            Velocity = directionToPlayer * _returnSpeed;
            
            transform.Translate(Velocity * Time.deltaTime);

            if (Vector2.Distance(transform.position, _owner.position) < 0.5f)
            {
                RemovePiece();
            }
        }
    }

    // --- GIAI ĐOẠN 2: DỪNG VÀ MỞ RỘNG ---
    IEnumerator HangRoutine()
    {
        _isHanging = true;
        transform.Translate(Velocity * 0.05f * Time.deltaTime);
        
        yield return new WaitForSeconds(_hangTime);

        _isReturning = true;
        _isHanging = false;
    }

    private void RemovePiece()
    {
        if (TimeController.Instance != null)
        {
            TimeController.Instance.UnregisterObject(this);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform == _owner) return;

        if (collision.TryGetComponent<IEnemy>(out var enemy))
        {
            observer.PostEvent(GameEvent.OnPlayerHit, (enemy, _damage));
            return;
        }
    }
}