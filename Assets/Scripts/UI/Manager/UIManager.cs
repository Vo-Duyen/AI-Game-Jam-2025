using System;
using DesignPattern;
using DesignPattern.Observer;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : Singleton<UIManager>
{
    [Title("UI Panels")] [SerializeField] private GameplayUI _gameplayUI;

    [SerializeField] private RestartUI _restartUI;
    [SerializeField] private HelpUI _helpUI;
    [SerializeField] private SettingUI _settingUI;

    [SerializeField] private WinUI _winUI;
    [SerializeField] private LoseUI _loseUI;

    private float _currentTimeScale;

    private ObserverManager<UIEventID> Observer => ObserverManager<UIEventID>.Instance;

    protected override void Awake()
    {
        DontDestroy = true;
        base.Awake();
    }

    private void OnEnable()
    {
        RegisterEvents();
        // ShowStartScreen();
        // ShowStartScreen();
        SoundManager.Instance.StopFX(SoundId.Background1);
        SoundManager.Instance.PlayFX(SoundId.Background2, true);
        SoundManager.Instance.PlayFX(SoundId.Wind);
    }

    private void OnDisable()
    {
        UnregisterEvents();
    }

    private void RegisterEvents()
    {
        Observer.RegisterEvent(UIEventID.OnRestartClicked, OnRestartClicked);
        Observer.RegisterEvent(UIEventID.OnCloseRestartClicked, OnCloseRestartClicked);
        Observer.RegisterEvent(UIEventID.OnRestartButtonClicked, OnRestartButtonClicked);

        Observer.RegisterEvent(UIEventID.OnHelpClicked, OnHelpClicked);
        Observer.RegisterEvent(UIEventID.OnCloseHelpClicked, OnCloseHelpClicked);

        Observer.RegisterEvent(UIEventID.OnSettingClicked, OnSettingClicked);
        Observer.RegisterEvent(UIEventID.OnCloseSettingClicked, OnCloseSettingClicked);

        Observer.RegisterEvent(UIEventID.OnStartGame, OnStartGame);
        Observer.RegisterEvent(UIEventID.OnWinGame, OnWinGame);
        Observer.RegisterEvent(UIEventID.OnLoseGame, OnLoseGame);

        Observer.RegisterEvent(UIEventID.OnNextLevelButtonClicked, OnNextLevelButtonClicked);
        Observer.RegisterEvent(UIEventID.OnTryAgainButtonClicked, OnTryAgainButtonClicked);
    }

    private void UnregisterEvents()
    {
        Observer.RemoveEvent(UIEventID.OnRestartClicked, OnRestartClicked);
        Observer.RemoveEvent(UIEventID.OnCloseRestartClicked, OnCloseRestartClicked);
        Observer.RemoveEvent(UIEventID.OnRestartButtonClicked, OnRestartButtonClicked);

        Observer.RemoveEvent(UIEventID.OnHelpClicked, OnHelpClicked);
        Observer.RemoveEvent(UIEventID.OnCloseHelpClicked, OnCloseHelpClicked);

        Observer.RemoveEvent(UIEventID.OnSettingClicked, OnSettingClicked);
        Observer.RemoveEvent(UIEventID.OnCloseSettingClicked, OnCloseSettingClicked);

        Observer.RemoveEvent(UIEventID.OnStartGame, OnStartGame);
        Observer.RemoveEvent(UIEventID.OnWinGame, OnWinGame);
        Observer.RemoveEvent(UIEventID.OnLoseGame, OnLoseGame);

        Observer.RemoveEvent(UIEventID.OnNextLevelButtonClicked, OnNextLevelButtonClicked);
        Observer.RemoveEvent(UIEventID.OnTryAgainButtonClicked, OnTryAgainButtonClicked);
    }

    #region Show Screens

    private void ShowStartScreen()
    {
        HideAllPanels();
    }

    private void ShowGameplayScreen()
    {
        HideAllPanels();
        _gameplayUI?.Show();
    }

    private void HideAllPanels()
    {
        _gameplayUI?.Hide(true);
        _restartUI?.Hide(true);
        _winUI?.Hide(true);
        _loseUI?.Hide(true);
    }

    #endregion

    public void UpdateLevel(int level)
    {
        _gameplayUI?.UpdateLevel(level);
    }

    public void OnRestartTimer(object param)
    {
        _gameplayUI.OnRestartTimeSlider(param);
    }

    #region Event Handlers

    private void OnRestartClicked(object param)
    {
        ButtonScaleAnim(param);
        SetContinueGame(false);
        _gameplayUI?.SetControl(false);
        _restartUI?.Show();
    }

    private void OnCloseRestartClicked(object param)
    {
        ButtonScaleAnim(param);
        SetContinueGame(true, 0.3f);
        _gameplayUI?.SetControl();
        _restartUI?.Hide();
    }

    private void OnRestartButtonClicked(object param)
    {
        ButtonScaleAnim(param);
        SetContinueGame(true, 0.3f);
        _gameplayUI?.SetControl();
        _restartUI?.Hide();
    }

    private void OnHelpClicked(object param)
    {
        ButtonScaleAnim(param);
        SetContinueGame(false);
        _gameplayUI?.SetControl(false);
        _helpUI?.Show();
    }

    private void OnCloseHelpClicked(object param)
    {
        ButtonScaleAnim(param);
        SetContinueGame(true, 0.3f);
        _gameplayUI?.SetControl();
        _helpUI?.Hide();
    }

    private void OnSettingClicked(object param)
    {
        ButtonScaleAnim(param);
        SetContinueGame(false);
        _gameplayUI?.SetControl(false);
        _settingUI?.Show();
    }

    private void OnCloseSettingClicked(object param)
    {
        ButtonScaleAnim(param);
        SetContinueGame(true, 0.3f);
        _gameplayUI?.SetControl();
        _settingUI?.Hide();
    }

    private void OnStartGame(object param)
    {
        ShowGameplayScreen();
    }

    private void OnWinGame(object param)
    {
        _gameplayUI?.SetControl(false);
        if (param is float timeDelay)
        {
            DOVirtual.DelayedCall(timeDelay, () =>
            {
                SetContinueGame(false);
                _winUI.Show();
            }).SetUpdate(true);
        }
        else
        {
            SetContinueGame(false);
            _winUI?.Show();
        }
    }

    private void OnLoseGame(object param)
    {
        _gameplayUI?.SetControl(false);
        if (param is float timeDelay)
        {
            DOVirtual.DelayedCall(timeDelay, () =>
            {
                SetContinueGame(false);
                _loseUI?.Show();
                SoundManager.Instance.PlayFX(SoundId.Lose);
            }).SetUpdate(true);
        }
        else
        {
            SetContinueGame(false);
            _loseUI?.Show();
            SoundManager.Instance.PlayFX(SoundId.Lose);
        }
    }

    private void OnNextLevelButtonClicked(object param)
    {
        ButtonScaleAnim(param);
        SetContinueGame(true, 0.3f);
        _gameplayUI?.SetControl();
        _winUI?.Hide();
    }

    private void OnTryAgainButtonClicked(object param)
    {
        ButtonScaleAnim(param);
        SetContinueGame(true, 0.3f);
        _gameplayUI?.SetControl();
        _loseUI?.Hide();
    }

    #endregion

    private void SetContinueGame(bool value = true, float timeDelay = 0f)
    {
        if (value)
        {
            Time.timeScale = _currentTimeScale;
        }
        else
        {
            _currentTimeScale = Time.timeScale;
            Time.timeScale = 0;
        }
    }

    private void ButtonScaleAnim(object param)
    {
        if (param is Transform target)
        {
            var scale = target.localScale;
            target.DOScale(scale * 1.2f, 0.1f).SetEase(Ease.Linear).SetLoops(2, LoopType.Yoyo).SetUpdate(true);
        }
    }

    public void ShowTip(bool isShow, string tip)
    {
        _gameplayUI?.ShowTip(isShow, tip);
    }
}