using System;
using System.Collections;
using System.Collections.Generic;
using DesignPattern.Observer;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class BaseUIPanel : SerializedMonoBehaviour
{
    [Title("Core")] [OdinSerialize] protected CanvasGroup canvasGroup;
    [OdinSerialize] protected float fadeTime = 0.3f;

    [OdinSerialize] private Dictionary<Transform, Vector3> _arrScale =
        new Dictionary<Transform, Vector3>();

    protected ObserverManager<UIEventID> Observer => ObserverManager<UIEventID>.Instance;

    public virtual void Show(bool immediate = false)
    {
        gameObject.SetActive(true);

        if (immediate)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            OnShowComplete();
        }
        else
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(1f, fadeTime).SetEase(Ease.OutBack).SetUpdate(true);
            foreach (var childScale in _arrScale)
            {
                childScale.Key.localScale = Vector3.zero;
                childScale.Key.DOScale(childScale.Value, fadeTime).SetEase(Ease.OutBack).SetUpdate(true);
            }

            OnDelayCall(fadeTime, () =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
                OnShowComplete();
            });
        }

        OnShow();
    }

    public virtual void Hide(bool immediate = false)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (immediate)
        {
            canvasGroup.alpha = 0f;
            gameObject.SetActive(false);
            OnHideComplete();
        }
        else
        {
            canvasGroup.DOKill();
            canvasGroup.DOFade(0f, fadeTime).SetEase(Ease.InBack).SetUpdate(true);
            foreach (var childScale in _arrScale)
            {
                childScale.Key.DOScale(0f, fadeTime).SetEase(Ease.InBack).SetUpdate(true);
            }

            OnDelayCall(fadeTime, () =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                gameObject.SetActive(false);
                OnHideComplete();
            });
        }

        OnHide();
    }

    public virtual void SetControl(bool value = true)
    {
        canvasGroup.blocksRaycasts = value;
    }

    protected virtual void OnShow()
    {
    }

    protected virtual void OnShowComplete()
    {
    }

    protected virtual void OnHide()
    {
    }

    protected virtual void OnHideComplete()
    {
    }

    protected virtual void OnDelayCall(float timeDelay, Action callback)
    {
        DOVirtual.DelayedCall(timeDelay, () => callback?.Invoke()).SetUpdate(true);
    }

#if UNITY_EDITOR
    [Button]
    protected virtual void SetupTest()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    [Button]
    protected virtual void SetupArrScale(Transform[] transforms)
    {
        foreach (var t in transforms)
        {
            _arrScale[t] = t.localScale;
        }
    }
#endif
}