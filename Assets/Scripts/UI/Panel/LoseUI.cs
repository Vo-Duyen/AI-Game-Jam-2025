using DesignPattern.Observer;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class LoseUI : BaseUIPanel
{
    [Title("Button")] [OdinSerialize] private Button _tryAgainButton;

    private void Awake()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        _tryAgainButton?.onClick.AddListener(OnTryAgainButtonClicked);
    }

    private void OnTryAgainButtonClicked()
    {
        Observer.PostEvent(UIEventID.OnTryAgainButtonClicked, _tryAgainButton.transform);
    }
}