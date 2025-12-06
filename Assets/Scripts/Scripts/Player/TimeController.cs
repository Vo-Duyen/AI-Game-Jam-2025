using System;
using DesignPattern;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : Singleton<TimeController>
{

    [Header("Settings")]
    public float MaxRecordTime = 5f;

    private Dictionary<TimeControlled, List<RecordFrameData>> _database = new Dictionary<TimeControlled, List<RecordFrameData>>();

    private List<TimeControlled> _registeredObjects = new List<TimeControlled>();

    public bool IsRewinding { get; private set; } = false;

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

    // --- LOGIC GHI ---
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
                _timestamp = currentTime
            });

            while (dataList.Count > 0 && (currentTime - dataList[0]._timestamp > MaxRecordTime))
            {
                dataList.RemoveAt(0);
            }

            obj.TimeUpdate();
        }
    }

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

                dataList.RemoveAt(lastIndex);
            }
        }

        if (!hasDataLeft)
        {
            StopRewind();
        }
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

    public Vector2 GetGhostPosition(TimeControlled obj)
    {
        if (_database.ContainsKey(obj) && _database[obj].Count > 0)
        {
            
            return _database[obj][0]._position;
        }
        return obj.transform.position;
    }
}
[System.Serializable]
public class RecordFrameData
{
    public Vector2 _position;
    public Vector2 _velocity;
    public float _timestamp;
}
