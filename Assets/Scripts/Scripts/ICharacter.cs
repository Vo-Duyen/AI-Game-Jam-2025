using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    Transform GetTransform();
    T GetType<T>() where T : Enum;
    bool IsType<T>(T type) where T : Enum;
    void ChangeType<T>(T type) where T : Enum;
    bool IsState<T>(T state) where T : Enum;
    void ChangeState<T>(T state) where T : Enum;
    void Reverse();
    void Jump();
    void Run();
    void Attack();
    void GetHit();
    void Die();
}

public interface IPlayer : ICharacter
{
    void Skill1();
    void Skill2();
    void Skill3();
}

public interface IEnemy : ICharacter
{
    bool IsCanMove(Vector3 direction);
    void Move(Vector3 direction, float speed);
    void Patrol();
    void Follow();
}