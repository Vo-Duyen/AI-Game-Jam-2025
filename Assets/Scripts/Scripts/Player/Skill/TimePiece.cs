using DesignPattern.ObjectPool;
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

    private Vector2 _startPos;
    private Transform _owner;
    
    private bool _isReturning = false;
    private bool _isHanging = false;

    public void Initialize(Transform owner, Vector2 direction)
    {
        _owner = owner;
        _startPos = transform.position;

        direction.Normalize();

        Velocity = direction * _throwSpeed;

        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

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

        if (collision.CompareTag("Enemy"))
        {
            Debug.Log("Hit Enemy: " + collision.name);
        }
    }
}