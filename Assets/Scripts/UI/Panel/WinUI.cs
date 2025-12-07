using DesignPattern.Observer;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WinUI : BaseUIPanel
{
    [Title("Display")] [OdinSerialize] private TextMeshProUGUI _titleText;
    [OdinSerialize] private TextMeshProUGUI _levelText;

    [Title("Buttons")] [OdinSerialize] private Button _nextLevelButton;


    private void Awake()
    {
        SetupButtons();
    }

    #region SetupButtons

    private void SetupButtons()
    {
        _nextLevelButton?.onClick.AddListener(OnNextLevelClicked);
    }

    private void OnNextLevelClicked()
    {
        Observer.PostEvent(UIEventID.OnNextLevelButtonClicked, _nextLevelButton.transform);
    }

    #endregion
}