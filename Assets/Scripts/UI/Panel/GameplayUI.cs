using System;
using DesignPattern.Observer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;


public class GameplayUI : BaseUIPanel
{
    [Title("Info Display")] [OdinSerialize]
    private TextMeshProUGUI _levelText;

    [OdinSerialize] private Slider _timeSlider;

    [Title("Buttons")] [OdinSerialize] private Button _restartButton;
    [OdinSerialize] private Button _helpButton;
    [OdinSerialize] private Button _settingButton;


    private Tween _tween;

    private Tween _timerTween;

    private void Awake()
    {
        SetupButtons();
    }

    public void OnRestartTimeSlider(object param)
    {
        if (param is float time)
        {
            _timeSlider.value = _timeSlider.maxValue;
            _tween?.Kill();
            _tween = DOVirtual.Float(0, time, time, t => { _timeSlider.value = (time - t) / time; })
                .SetEase(Ease.Linear).OnComplete(() => { Observer.PostEvent(UIEventID.OnLoseGame); });
        }
    }

    private void SetupButtons()
    {
        _restartButton?.onClick.AddListener(OnRestartClicked);
        _helpButton?.onClick.AddListener(OnHelpClicked);
        _settingButton?.onClick.AddListener(OnSettingClicked);
    }

    #region Button Handlers

    private void OnRestartClicked()
    {
        Observer.PostEvent(UIEventID.OnRestartClicked, _restartButton.transform);
    }

    private void OnHelpClicked()
    {
        Observer.PostEvent(UIEventID.OnHelpClicked, _helpButton.transform);
    }

    private void OnSettingClicked()
    {
        Observer.PostEvent(UIEventID.OnSettingClicked, _settingButton.transform);
    }

    #endregion

    public void UpdateLevel(int level)
    {
        // _timerTween?.Kill();
        // _timerTween = _levelText.transform.DOPunchScale(Vector3.one * 0.5f, 0.5f);
    }

    public void ShowTip(bool isShow, string tip)
    {
        // _tipText.text = tip;
        // _tipText.gameObject.SetActive(isShow);
    }
}