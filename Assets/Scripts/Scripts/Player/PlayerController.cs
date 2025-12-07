using DesignPattern.ObjectPool;
using DesignPattern.Observer;
using LongNC;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class PlayerController : TimeControlled, IPlayer
{
    [Header("Movement Components")]
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private Animator _animator;
    [SerializeField] private AnimationClip _animGetHit;
    [SerializeField] private AnimationClip _animDie;

    [Header("Stats")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _damage = 10f;
    [SerializeField] private float _maxHealth = 100f;
    [SerializeField] private float _curHealth = 100f;
    [SerializeField] private float _range;
    [Header("Skill 2: TimePiece (Q)")]
    [SerializeField] private TimePiece _timePiecePrefab;

    [Header("Skill 3: Chronobreak (R)")]
    [SerializeField] private SpriteRenderer _ghostVFX;
    [SerializeField] private GameObject _explosionVFX;
    [SerializeField] private float _ultRadius = 3.5f;
    [SerializeField] private int _ultDamage = 150;

    [Header("Countdown")]
    [SerializeField] private float _skill1Cooldown = 5f;
    [SerializeField] private float _skill2Cooldown = 3f;
    [SerializeField] private float _skill3Cooldown = 10f;
    [SerializeField] private float _attackCooldown = 0.5f;
    [SerializeField] private Image _skill1CooldownImg;
    [SerializeField] private Image _skill2CooldownImg;
    [SerializeField] private Image _skill3CooldownImg;

    [SerializeField] protected List<SpriteRenderer> _arrSprites = new  List<SpriteRenderer>();
    [SerializeField] protected float _animGetHitTime = 0.1f;
    [SerializeField] private Image healthBarImg;
    protected ObserverManager<GameEvent> observer => ObserverManager<GameEvent>.Instance;
    
    private float _skill1Timer;
    private float _skill2Timer;
    private float _skill3Timer;
    private float _attackTimer;
    private float _horizontal;
    [SerializeField] private Transform _hitPoint;
    protected Collider2D[] _cols = new Collider2D[10];

    private void OnEnable()
    {
        observer.RegisterEvent(GameEvent.OnEnemyHit, OnEnemyHit);
    }

    private void OnEnemyHit(object param)
    {
        if (param is (IPlayer player, float damage))
        {
            if (player == (IPlayer)this)
            {
                GetHit();
                _curHealth -= damage;
                if (_curHealth <= 0)
                {
                    _curHealth = 0;
                }
                UpdateHealthBar();
                if (_curHealth <= 0)
                {
                    ObserverManager<UIEventID>.Instance.PostEvent(UIEventID.OnLoseGame);
                }
            }
        }
    }

    private void Update()
    {
        if (_skill1Timer > 0) _skill1Timer -= Time.deltaTime;
        if (_skill2Timer > 0) _skill2Timer -= Time.deltaTime;
        if (_skill3Timer > 0) _skill3Timer -= Time.deltaTime;

        if (_attackTimer > 0) _attackTimer -= Time.deltaTime;

        UpdateCooldownUI();

        if (Input.GetKeyDown(KeyCode.Q) && _skill1Timer <= 0) ((IPlayer)this).Skill1();
        if (Input.GetKeyUp(KeyCode.Q) && _skill1Timer <= 0)
        {
            TimeController.Instance.StopRewind();
            _skill1Timer = _skill1Cooldown;
        }

        if (Input.GetKeyDown(KeyCode.E) && _skill2Timer <= 0)
        {
            ((IPlayer)this).Skill2();
            _skill2Timer = _skill2Cooldown;
        }

        if (Input.GetKeyDown(KeyCode.R) && _skill3Timer <= 0)
        {
            ((IPlayer)this).Skill3();
            _skill3Timer = _skill3Cooldown;
        }

        if (Input.GetMouseButtonDown(0) && _attackTimer <= 0)
        {
            Attack();
            _attackTimer = _attackCooldown;
        }

        UpdateGhostVisual();
    }

    private void UpdateCooldownUI()
    {
        if (_skill1CooldownImg != null)
            _skill1CooldownImg.fillAmount = _skill1Timer / _skill1Cooldown;

        if (_skill2CooldownImg != null)
            _skill2CooldownImg.fillAmount = _skill2Timer / _skill2Cooldown;

        if (_skill3CooldownImg != null)
            _skill3CooldownImg.fillAmount = _skill3Timer / _skill3Cooldown;
    }

    [FoldoutGroup("Setup arr sprites"), Button]
    protected virtual void GetSprites()
    {
        _arrSprites.Clear();
        var queue = new Queue<Transform>();
        queue.Enqueue(transform);
        while (queue.Count > 0)
        {
            var target = queue.Dequeue();
            if (target.TryGetComponent<SpriteRenderer>(out var spriteRenderer))
            {
                _arrSprites.Add(spriteRenderer);
            }

            foreach (Transform child in target)
            {
                queue.Enqueue(child);
            }
        }
    }

    public void UpdateHealthBar()
    {
        if (healthBarImg != null)
        {
            healthBarImg.fillAmount = _curHealth / _maxHealth;
        }
    }    

    public override void TimeUpdate()
    {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _playerMovement.Move(_horizontal, _moveSpeed);

        if (Input.GetKeyDown(KeyCode.W) && _groundChecker.GroundChecked())
        {
            _playerMovement.Jump(_jumpForce);
        }
    }

    void IPlayer.Skill1()
    {
        SoundManager.Instance.PlayFX(SoundId.Skill1);
        TimeController.Instance.StartRewind();
    }

    void IPlayer.Skill2()
    {

        SoundManager.Instance.PlayFX(SoundId.Skill2);
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        Vector2 shootDirection = (mouseWorldPos - transform.position).normalized;

        TimePiece newPiece = Instantiate(_timePiecePrefab, transform.position, Quaternion.identity);
        newPiece.Initialize(this.transform, shootDirection);
    }

    public override float GetCurrentHealth()
    {
        return _curHealth;
    }

    void IPlayer.Skill3()
    {
        SoundManager.Instance.PlayFX(SoundId.Skill3);
        RecordFrameData ghostFrame = TimeController.Instance.GetGhostFrame(this);

        if (ghostFrame != null)
        {
            transform.position = ghostFrame._position;

            _curHealth = ghostFrame._health;
            UpdateHealthBar();
        }
        else
        {
            transform.position = TimeController.Instance.GetGhostPosition(this);
        }

        _playerMovement.Rigidbody2D.velocity = Vector2.zero;
        Velocity = Vector2.zero;
        _skill1Cooldown = 0f;
        _skill2Cooldown = 0f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _ultRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                observer.PostEvent(GameEvent.OnPlayerHit, (hit.GetComponent<IEnemy>(), (float)_ultDamage));
                Debug.Log("Ulti Damage: " + hit.name);
            }
        }
    }

    private void UpdateGhostVisual()
    {
        if (_skill3Timer > 0)
        {
            if (_ghostVFX.gameObject.activeSelf) _ghostVFX.gameObject.SetActive(false);
            return;
        }

        RecordFrameData ghostFrame = TimeController.Instance.GetGhostFrame(this);

        if (ghostFrame != null)
        {
            float dataAge = Time.time - ghostFrame._timestamp;
            float maxTime = TimeController.Instance.MaxRecordTime;

            if (dataAge >= maxTime - 0.1f)
            {
                if (!_ghostVFX.gameObject.activeSelf) _ghostVFX.gameObject.SetActive(true);

                _ghostVFX.transform.position = ghostFrame._position;
                _ghostVFX.sprite = ghostFrame._sprite;

                _ghostVFX.transform.localScale = ghostFrame._localScale;
            }
            else
            {
                if (_ghostVFX.gameObject.activeSelf) _ghostVFX.gameObject.SetActive(false);
            }

            float distance = Vector2.Distance(ghostFrame._position, transform.position);
            if (distance < 0.5f) _ghostVFX.gameObject.SetActive(false);
        }
        else
        {
            if (_ghostVFX.gameObject.activeSelf) _ghostVFX.gameObject.SetActive(false);
        }
    }

    void ICharacter.Reverse() { }
    void ICharacter.Jump() { }
    void ICharacter.Run() { }
    public void Attack()
    {
        SoundManager.Instance.PlayFX(SoundId.KnifeAttack);
        _animator.SetTrigger("IsAttacking");
        var size = Physics2D.OverlapCircleNonAlloc(_hitPoint.position, _range, _cols);
        for (var i = 0; i < size; ++i)
        {
            var target = _cols[i].gameObject;
            if (target.TryGetComponent<IEnemy>(out var enemy))
            {
                observer.PostEvent(GameEvent.OnPlayerHit, (enemy, _damage));
                return;
            }
        }
    }
    public void GetHit()
    {
        SoundManager.Instance.PlayFX(SoundId.GetHit1);
        var red = Color.red;
        red.a = 0.5f;
        for (var i = 0; i < _arrSprites.Count; ++i)
        {
            _arrSprites[i].color = red;
        }
        OnDelayCall(_animGetHitTime, () =>
        {
            for (var i = 0; i < _arrSprites.Count; ++i)
            {
                _arrSprites[i].color = Color.white;
            }
        });
    }
    public void Die()
    {
        SoundManager.Instance.PlayFX(SoundId.EnemyDie);
        if (_animDie == null) return;
        _animator.Play(_animDie.name);
        
        // TODO: Fix late
        OnDelayCall(_animDie.length + 0.1f, () =>
        {
            PoolingManager.Despawn(gameObject);
        });
    }
    T ICharacter.GetType<T>() { throw new System.NotImplementedException(); }
    bool ICharacter.IsType<T>(T type) { throw new System.NotImplementedException(); }
    void ICharacter.ChangeType<T>(T type) { }
    bool ICharacter.IsState<T>(T state) { throw new System.NotImplementedException(); }
    void ICharacter.ChangeState<T>(T state) { }

    public Transform GetTransform()
    {
        return this.transform;
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

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        DrawCircle(_hitPoint.position, _range, 60);
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

    private void OnDisable()
    {
        observer.RemoveEvent(GameEvent.OnEnemyHit, OnEnemyHit);
    }
}