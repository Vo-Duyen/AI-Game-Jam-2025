using System;
using DesignPattern;
using DesignPattern.Observer;

public enum GameEvent
{
    OnPlayerHit,
    OnEnemyHit,
    Reverse,
}

public class GameManager : Singleton<GameManager>
{
    protected ObserverManager<GameEvent> observer => ObserverManager<GameEvent>.Instance;

    private void Start()
    {
        // observer.PostEvent(GameEvent.Reverse, 5f);
    }
}
