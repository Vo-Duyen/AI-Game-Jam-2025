using System;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;

namespace LongNC
{
    public class ItemAlpha : SerializedMonoBehaviour
    {
        [OdinSerialize] private List<Material> _materials = new List<Material>();
        [OdinSerialize, OnValueChanged("LateUpdate")] private float _alpha = 1f;

        private List<float> _alphaList = new List<float>();
        private Color _color;
        private float a = -1;

        [Button]
        public void Setup(bool isClearArr = true)
        {
            if (isClearArr)
            {
                _materials.Clear();
                _alphaList.Clear();
            }
            
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(transform);
            while (queue.Count > 0)
            {
                Transform target = queue.Dequeue();

                for (var i = 0; i < target.childCount; ++i)
                {
                    queue.Enqueue(target.GetChild(i));
                }

                if (target.TryGetComponent<MeshRenderer>(out var meshRenderer))
                {
                    _materials.Add(meshRenderer.sharedMaterial);
                    _alphaList.Add(meshRenderer.sharedMaterial.color.a);
                }
            }
        }

        [Button]
        public void SetAlpha(float alpha)
        {
            _alpha = alpha;
            LateUpdate();
        }

        [Button]
        public void DoAlpha(float alpha, float time, float delay = 0f)
        {
            if (_materials.Count == 0 || _alphaList.Count != _materials.Count)
            {
                Debug.LogWarning("Call Setup() first!");
                return;
            }
            
            for (var i = 0; i < _materials.Count; ++i)
            {
                _color = _materials[i].color;
                _color.a = _alphaList[i] * alpha;
                _materials[i].DOColor(_color, time).SetDelay(delay);
            }
        }

        private void LateUpdate()
        {
            if (a != _alpha)
            {
                a = _alpha;
                
                if (_materials.Count == 0 || _alphaList.Count != _materials.Count)
                {
                    return;
                }
                
                for (var i = 0; i < _materials.Count; ++i)
                {
                    _color = _materials[i].color;
                    _color.a = _alphaList[i] * a;
                    _materials[i].color = _color;
                }
            }
        }
    }
}