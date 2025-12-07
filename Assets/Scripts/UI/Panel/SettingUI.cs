using System;
using DesignPattern.Observer;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;


public class SettingUI : BaseUIPanel
{
    [Title("Sound")] [OdinSerialize] private Slider _sliderSound;

    [Title("Close Settings")] [OdinSerialize]
    private Button _closeButton;

    private Tween _tween;

    private void Awake()
    {
        Setup();
    }

    #region Setup

    private void Setup()
    {
        SetupButtons();
        SetupSlider();
    }

    private void SetupButtons()
    {
        _closeButton?.onClick.AddListener(OnCloseButtonClicked);
    }

    private void SetupSlider()
    {
        var volumeAll = PlayerPrefs.GetFloat("VolumeAll");
        var checkVolume = PlayerPrefs.GetString("CheckVolume");
        if (checkVolume != "true")
        {
            volumeAll = 1;
        }

        if (volumeAll < 0 || volumeAll > 1)
        {
            volumeAll = 1;
        }

        _sliderSound.value = volumeAll;
        _sliderSound?.onValueChanged.AddListener(OnSliderValueChanged);
    }

    #endregion

    #region Handle

    private void OnCloseButtonClicked()
    {
        Observer.PostEvent(UIEventID.OnCloseSettingClicked, _closeButton.transform);
    }

    private void OnSliderValueChanged(float value)
    {
        Observer.PostEvent(UIEventID.OnSoundChanged, value);
    }

    #endregion
}