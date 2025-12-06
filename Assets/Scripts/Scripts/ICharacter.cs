using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    public T GetType<T>() where T : Enum;
    public bool IsType<T>(T type) where T : Enum;
    public void ChangeType<T>(T type) where T : Enum;
    public bool IsState<T>(T state) where T : Enum;
    public void ChangeState<T>(T state) where T : Enum;
    public void Reverse();
    public void Jump();
    public void Run();
    public void Attack();
    public void GetHit();
    public void Die();
}

public interface IPlayer : ICharacter
{
    public void Skill1();
    public void Skill2();
    public void Skill3();
}

public interface IEnemy : ICharacter
{
    
}
