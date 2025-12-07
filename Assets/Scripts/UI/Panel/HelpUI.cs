using System;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class HelpUI : BaseUIPanel
{
    [Title("Close Settings")] [OdinSerialize]
    private Button _closeButton;

    [OdinSerialize] private Button _thankButton;

    private void Awake()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        _closeButton?.onClick.AddListener(OnCloseButtonClicked);
        _thankButton?.onClick.AddListener(OnCloseButtonClicked);
    }

    private void OnCloseButtonClicked()
    {
        Observer.PostEvent(UIEventID.OnCloseHelpClicked, _closeButton);
    }
}