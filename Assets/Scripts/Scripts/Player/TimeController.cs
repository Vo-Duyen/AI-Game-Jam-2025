using System;
using DesignPattern; // Nếu bạn dùng namespace này, còn không thì xóa đi
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour // Hoặc Singleton<TimeController>
{
    public static TimeController Instance { get; private set; }

    [Header("Settings")]
    public float MaxRecordTime = 5f;

    private Dictionary<TimeControlled, List<RecordFrameData>> _database = new Dictionary<TimeControlled, List<RecordFrameData>>();
    private List<TimeControlled> _registeredObjects = new List<TimeControlled>();

    public bool IsRewinding { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        TimeControlled[] foundObjects = FindObjectsOfType<TimeControlled>();
        foreach (var obj in foundObjects)
        {
            RegisterObject(obj);
        }
    }

    private void Update()
    {
        if (IsRewinding)
            Rewind();
        else
            Record();
    }

    // --- LOGIC GHI (SỬA ĐỔI) ---
    private void Record()
    {
        float currentTime = Time.time;

        for (int i = 0; i < _registeredObjects.Count; i++)
        {
            TimeControlled obj = _registeredObjects[i];
            if (obj == null) continue;
            if (!_database.ContainsKey(obj)) continue;

            List<RecordFrameData> dataList = _database[obj];

            dataList.Add(new RecordFrameData
            {
                _position = obj.transform.position,
                _velocity = obj.Velocity,
                _timestamp = currentTime,
                _sprite = (obj.MySpriteRenderer != null) ? obj.MySpriteRenderer.sprite : null,
                _localScale = obj.transform.localScale,
                _health = obj.GetCurrentHealth()
            });

            while (dataList.Count > 0 && (currentTime - dataList[0]._timestamp > MaxRecordTime))
            {
                dataList.RemoveAt(0);
            }

            obj.TimeUpdate();
        }
    }

    // --- LOGIC TUA (SỬA ĐỔI) ---
    private void Rewind()
    {
        bool hasDataLeft = false;

        foreach (var obj in _registeredObjects)
        {
            if (obj == null) continue;
            List<RecordFrameData> dataList = _database[obj];

            if (dataList.Count > 0)
            {
                hasDataLeft = true;

                int lastIndex = dataList.Count - 1;
                RecordFrameData data = dataList[lastIndex];

                obj.transform.position = data._position;
                obj.Velocity = data._velocity;

                if (obj.MySpriteRenderer != null && data._sprite != null)
                {
                    obj.MySpriteRenderer.sprite = data._sprite;
                }
                obj.transform.localScale = data._localScale;

                dataList.RemoveAt(lastIndex);
            }
        }

        if (!hasDataLeft) StopRewind();
    }

    public void StartRewind() => IsRewinding = true;
    public void StopRewind() => IsRewinding = false;

    public void RegisterObject(TimeControlled obj)
    {
        if (!_registeredObjects.Contains(obj))
        {
            _registeredObjects.Add(obj);
            _database[obj] = new List<RecordFrameData>();
        }
    }

    public void UnregisterObject(TimeControlled obj)
    {
        if (_registeredObjects.Contains(obj))
        {
            _registeredObjects.Remove(obj);
            _database.Remove(obj);
        }
    }

    public RecordFrameData GetGhostFrame(TimeControlled obj)
    {
        if (_database.ContainsKey(obj) && _database[obj].Count > 0)
        {
            return _database[obj][0];
        }
        return null;
    }

    public Vector2 GetGhostPosition(TimeControlled obj)
    {
        var frame = GetGhostFrame(obj);
        return frame != null ? frame._position : (Vector2)obj.transform.position;
    }
}

[System.Serializable]
public class RecordFrameData
{
    public Vector2 _position;
    public Vector2 _velocity;
    public float _timestamp;
    public float _health;
    public Sprite _sprite;
    public Vector3 _localScale;
}
