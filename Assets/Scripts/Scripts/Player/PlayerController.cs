using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour, IPlayer
{
    [Header("Player Stat")]
    private float horizontal;
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _jumpForce = 8f;

    [Header("Components")]
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private GroundChecker _groundChecker;

    private float _horizontal;

    private void Start()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {

        horizontal = Input.GetAxisRaw("Horizontal");
        _playerMovement.Move(horizontal, _moveSpeed);
        if (Input.GetKeyDown(KeyCode.W) && _groundChecker.GroundChecked())
        {
            _playerMovement.Jump(_jumpForce);
        }
    }
    void IPlayer.Skill1()
    {
        
    }

    void IPlayer.Skill2()
    {
        
    }

    void IPlayer.Skill3()
    {
        
    }
    void ICharacter.Reverse()
    {
        throw new System.NotImplementedException();
    }

    void ICharacter.Jump()
    {
        throw new System.NotImplementedException();
    }

    void ICharacter.Run()
    {
        throw new System.NotImplementedException();
    }

    void ICharacter.Attack()
    {
        throw new System.NotImplementedException();
    }

    void ICharacter.GetHit()
    {
        throw new System.NotImplementedException();
    }

    void ICharacter.Die()
    {
        throw new System.NotImplementedException();
    }

    T ICharacter.GetType<T>()
    {
        throw new System.NotImplementedException();
    }

    void ICharacter.SetType<T>(T type)
    {
        throw new System.NotImplementedException();
    }

    T ICharacter.GetState<T>()
    {
        throw new System.NotImplementedException();
    }

    void ICharacter.SetState<T>(T state)
    {
        throw new System.NotImplementedException();
    }

}
