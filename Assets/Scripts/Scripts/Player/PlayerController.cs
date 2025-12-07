using DesignPattern.ObjectPool;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerController : TimeControlled, IPlayer
{
    [Header("Movement Components")]
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private GroundChecker _groundChecker;
    [SerializeField] private Animator _animator;

    [Header("Stats")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 8f;
    [SerializeField] private float _damage = 10f;

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

    private float _skill1Timer;
    private float _skill2Timer;
    private float _skill3Timer;
    private float _horizontal;
    

    private void Update()
    {
        if (_skill1Timer > 0) _skill1Timer -= Time.deltaTime;
        if (_skill2Timer > 0) _skill2Timer -= Time.deltaTime;
        if (_skill3Timer > 0) _skill3Timer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Q) && _skill1Timer <= 0) ((IPlayer)this).Skill1();
        if (Input.GetKeyUp(KeyCode.Q))
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

        if (Input.GetMouseButtonDown(0))
        {
            ((IPlayer)this).Attack();
        }

        UpdateGhostVisual();
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
        TimeController.Instance.StartRewind();
    }

    void IPlayer.Skill2()
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        Vector2 shootDirection = (mouseWorldPos - transform.position).normalized;

        TimePiece newPiece = Instantiate(_timePiecePrefab, transform.position, Quaternion.identity);
        newPiece.Initialize(this.transform, shootDirection);
    }

    void IPlayer.Skill3()
    {
        Vector2 targetPos = TimeController.Instance.GetGhostPosition(this);

        transform.position = targetPos;
        _playerMovement.Rigidbody2D.velocity = Vector2.zero;
        Velocity = Vector2.zero;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _ultRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Debug.Log("Ulti Damage: " + hit.name);
            }
        }

        // 4. Effect nổ
        //if (_explosionVFX) PoolingManager.Spawn(_explosionVFX, transform.position, Quaternion.identity);
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
    void ICharacter.Attack() 
    { 
        _animator.SetTrigger("IsAttacking");
    }
    void ICharacter.GetHit() 
    {
        
    }
    void ICharacter.Die() 
    {

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
}