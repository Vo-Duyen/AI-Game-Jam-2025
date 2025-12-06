using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacter
{
    public T GetType<T>();
    public void SetType<T>(T type);
    public T GetState<T>();
    public void SetState<T>(T state);
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
    public void Skill();
}
