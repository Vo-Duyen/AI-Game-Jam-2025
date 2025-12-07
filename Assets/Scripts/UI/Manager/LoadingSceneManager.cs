using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : SerializedMonoBehaviour
{
    [OdinSerialize] protected GameObject _nameGame;
    [OdinSerialize] protected Slider _loading;
    [OdinSerialize] protected float _timeLoading = 3f;
    [OdinSerialize] protected Button _playButton;
    
    protected List<Tweener> _tweeners = new List<Tweener>();

    protected AsyncOperation _loadingAsync;
    
    protected List<Vector3> _scaleValues = new List<Vector3>
    {
        new Vector3(1.2f, 0.8f, 1),
        new Vector3(0.9f, 1.1f, 1),
        Vector3.one,
    };
    
    protected virtual void Start()
    {
        SetupNameGame(true);
        SetupLoading();
        SetupButtons();
        DOVirtual.DelayedCall(_timeLoading + 0.1f, PlayActive);
        SoundManager.Instance.PlayFX(SoundId.Background1, true);
    }

    #region Setup

    protected virtual void SetupNameGame(bool isLoop)
    {
        _tweeners.Add(_nameGame.transform.DOScale(Vector3.one * 1.25f, 1f).SetLoops(isLoop ? -1 : 1, LoopType.Yoyo));
    }

    protected virtual void SetupLoading()
    {
        _loading.DOValue(1f, _timeLoading);
        _loadingAsync = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
        if (_loadingAsync != null) _loadingAsync.allowSceneActivation = false;
    }

    protected virtual void SetupButtons()
    {
        _playButton.gameObject.SetActive(false);
        _playButton.onClick.AddListener(() =>
        {
            _loadingAsync.allowSceneActivation = true;
        });
    }
    
    #endregion


    protected virtual void PlayActive()
    {
        _playButton.gameObject.SetActive(true);
        _loading.gameObject.SetActive(false);
        _tweeners.Add(_playButton.gameObject.transform.DOScale(Vector3.one * 1.5f, 1f).SetLoops(-1, LoopType.Yoyo));
    }

    protected virtual void OnDisable()
    {
        foreach (var t in _tweeners)
        {
            t.Kill();
        }
    }
}
