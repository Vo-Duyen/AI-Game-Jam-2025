using System;
using DesignPattern;
using System.Collections;
using DesignPattern;
using DesignPattern.Observer;
using DG.Tweening;
using UnityEngine;
using DesignPattern.Observer;
using UnityEngine.SceneManagement;

public enum GameEvent
{
    OnPlayerHit,
    OnEnemyHit,
    Reverse,
}


public class GameManager : Singleton<GameManager>
{
    private ObserverManager<UIEventID> Observer => ObserverManager<UIEventID>.Instance;

    private int _curLevel;

    private void OnEnable()
    {
        RegisterObserver();
        _curLevel = PlayerPrefs.GetInt("CurrentLevel");
        if (_curLevel <= 0) _curLevel = 1;
        UIManager.Instance.UpdateLevel(_curLevel);
    }

    private void OnDisable()
    {
        UnregisterObserver();
    }

    #region Observers

    private void RegisterObserver()
    {
        Observer.RegisterEvent(UIEventID.OnRestartButtonClicked, OnRestartButtonClicked);
        Observer.RegisterEvent(UIEventID.OnTryAgainButtonClicked, OnTryAgainButtonClicked);
    }

    private void UnregisterObserver()
    {
        Observer.RemoveEvent(UIEventID.OnRestartButtonClicked, OnRestartButtonClicked);
        Observer.RemoveEvent(UIEventID.OnTryAgainButtonClicked, OnTryAgainButtonClicked);
    }

    #endregion

    #region Button Handlers
    

    private void OnRestartButtonClicked(object param)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnTryAgainButtonClicked(object param)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    #endregion

    public void ResetLevel()
    {
        PlayerPrefs.SetInt("CurrentLevel", 0);
        OnRestartButtonClicked(null);
    }
}