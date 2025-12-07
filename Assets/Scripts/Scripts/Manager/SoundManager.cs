using System;
using System.Collections.Generic;
using DesignPattern;
using DesignPattern.Observer;
using DG.Tweening;
using UnityEngine;

public enum SoundId
{
    Background1,
    Background2,
    Win,
    Lose,
    EnemyDie,
    Wind,
    KnifeAttack,
    GetHit1,
    GetHit2,
    Skill1,
    Skill2,
    Skill3,
}

public class SoundManager : Singleton<SoundManager>
{
    private Dictionary<SoundId, AudioSource> _audioDict = new Dictionary<SoundId, AudioSource>();

    [SerializeField] private List<AudioClip> _soundId = new List<AudioClip>();

    private ObserverManager<UIEventID> Observer => ObserverManager<UIEventID>.Instance;

    protected override void Awake()
    {
        DontDestroy = true;
        base.Awake();
    }

    private void OnEnable()
    {
        Observer.RegisterEvent(UIEventID.OnSoundChanged, OnSoundChange);
    }

    private void OnDisable()
    {
        Observer.RemoveEvent(UIEventID.OnSoundChanged, OnSoundChange);
    }

    private void OnSoundChange(object param)
    {
        if (param is float value)
        {
            SetVolume(value);
        }
    }

    public void PlayFX(SoundId soundId, float timeDelay, bool isLoop = false)
    {
        DOVirtual.DelayedCall(timeDelay, () => { PlayFX(soundId, isLoop); }).SetUpdate(true);
    }

    public void PlayFX(SoundId soundId, bool isLoop = false)
    {
        if (!_audioDict.ContainsKey(soundId))
        {
            _audioDict[soundId] = gameObject.AddComponent<AudioSource>();
            _audioDict[soundId].clip = _soundId[(int)soundId];
        }

        var volume = PlayerPrefs.GetFloat("VolumeAll");
        var isMute = PlayerPrefs.GetString("IsMute");
        if (isMute != "true")
        {
            volume = 1;
            PlayerPrefs.SetString("IsMute", "true");
        }

        if (volume < 0 || volume > 1)
        {
            volume = 1;
        }

        _audioDict[soundId].volume = volume;
        _audioDict[soundId].loop = isLoop;
        _audioDict[soundId].Play();
    }

    public bool IsPlaying(SoundId soundId)
    {
        return _audioDict.ContainsKey(soundId) && _audioDict[soundId].isPlaying;
    }

    public void StopFX(SoundId soundId)
    {
        if (!_audioDict.ContainsKey(soundId))
        {
            Debug.LogWarning($"Sound {soundId} not found!");
            return;
        }

        _audioDict[soundId].Stop();
    }

    public void SetVolume(float value)
    {
        PlayerPrefs.SetFloat("VolumeAll", value);
        foreach (var audioSource in _audioDict)
        {
            audioSource.Value.volume = value;
        }
    }
}