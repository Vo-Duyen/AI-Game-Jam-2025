using System;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPattern.Observer
{
    public class ObserverManager<T> where T : Enum
    {
        private static ObserverManager<T> _instance;

        public static ObserverManager<T> Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ObserverManager<T>();
                }
                return _instance;
            }
        }

        private Dictionary<T, Action<object>> _events = new Dictionary<T, Action<object>>();

        private ObserverManager() { }

        public void RegisterEvent(T eventID, Action<object> callback)
        {
            if (callback == null)
            {
                Debug.LogWarning($"Callback cho sự kiện '{eventID}' là NULL.");
                return;
            }

            if (eventID == null)
            {
                Debug.LogWarning($"EventID là NULL. Không thể đăng ký sự kiện.");
                return;
            }

            if (!_events.TryAdd(eventID, callback))
            {
                _events[eventID] += callback;
            }
        }

        public void RemoveEvent(T eventID, Action<object> callback)
        {
            if (_events.ContainsKey(eventID))
            {
                _events[eventID] -= callback;
                if (_events[eventID] == null)
                {
                    _events.Remove(eventID);
                }
            }
            else
            {
                Debug.LogWarning($"Sự kiện '{eventID}' không tìm thấy trong ObserverManager<{typeof(T).Name}>");
            }
        }

        public void RemoveAllEvent()
        {
            _events.Clear();
        }

        public void PostEvent(T eventID, object param = null)
        {
            if (!_events.ContainsKey(eventID))
            {
                Debug.LogWarning($"Sự kiện '{eventID}' không có người nghe trong ObserverManager<{typeof(T).Name}>");
                return;
            }

            if (_events[eventID] == null)
            {
                Debug.LogWarning($"Callback cho sự kiện '{eventID}' là NULL.");
                _events.Remove(eventID);
                return;
            }

            _events[eventID]?.Invoke(param);
        }
    }
}