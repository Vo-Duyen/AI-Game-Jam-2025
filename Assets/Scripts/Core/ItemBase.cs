using System;
using System.Collections;
using System.Linq;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace LongNC
{
    public class ItemBase<TState, TType> : MonoBehaviour 
        where TState : Enum 
        where TType : Enum
    {
        private const string SetupString = "Set up";
        private const string AddComponentString = "Set up/Add component";
        private const string ComponentString = "Set up/Component";
        
        public Transform TF => transform;
        public int Order => _sortingGroup != null ? _sortingGroup.sortingOrder : -100000;
        
        [BoxGroup(SetupString), SerializeField] protected TState _state;
        [BoxGroup(SetupString), SerializeField] protected TType _type;
        [FoldoutGroup(AddComponentString), SerializeField] protected bool _isSortingGroup;
        [FoldoutGroup(AddComponentString), SerializeField] protected bool _isBoxCollider;
        [FoldoutGroup(AddComponentString), SerializeField] protected bool _isSphereCollider;
        [FoldoutGroup(AddComponentString), SerializeField] protected bool _isAnimation;
        [FoldoutGroup(AddComponentString), SerializeField] protected bool _isItemAlpha;
        [FoldoutGroup(AddComponentString), SerializeField] protected bool _isMeshRenderer = true;
        [BoxGroup(ComponentString), SerializeField] protected SortingGroup _sortingGroup;
        [BoxGroup(ComponentString), SerializeField] protected Collider _collider;
        [BoxGroup(ComponentString), SerializeField] protected Animation _animation;
        [BoxGroup(ComponentString), SerializeField] protected AnimationClip _animationClipScale;
        [BoxGroup(ComponentString), SerializeField] protected ItemAlpha _itemAlpha;
        [BoxGroup(ComponentString), SerializeField] protected MeshRenderer _meshRenderer;
        
        [Title("Pos in X")]
        [SerializeField] protected Vector3 _posInSlot;
        
        [FoldoutGroup(AddComponentString), Button]
        protected virtual void Setup()
        {
            if (TryGetComponent<SortingGroup>(out var sg))
            {
                _sortingGroup = sg;
            }
            else if (_isSortingGroup)
            {
                _sortingGroup = gameObject.AddComponent<SortingGroup>();
            }

            if (TryGetComponent<BoxCollider>(out var col))
            {
                _collider = col;
            }
            else if (_isBoxCollider)
            {
                _collider = gameObject.AddComponent<BoxCollider>();
            }
            else if (_isSphereCollider)
            {
                _collider = gameObject.AddComponent<SphereCollider>();
            }

            if (TryGetComponent<Animation>(out var a))
            {
                _animation = a;
            }
            else if (_isAnimation)
            {
                _animation = gameObject.AddComponent<Animation>();
            }
            
            if (TryGetComponent<ItemAlpha>(out var alpha))
            {
                _itemAlpha = alpha;
            }
            else if (_isItemAlpha)
            {
                _itemAlpha = gameObject.AddComponent<ItemAlpha>();
            }

            if (_isMeshRenderer)
            {
                _meshRenderer = GetComponentInChildren<MeshRenderer>();
            }
        }
        
        public virtual bool IsState<T>(T t)
        {
            if (t is TState state) return _state.Equals(state);
            Debug.LogWarning($"{name} : {t}");
            return false;
        }
        
        public virtual void ChangeState<T>(T t)
        {
            if (t is not TState s)
            {
                Debug.LogError($"{GetType().Name} should be of type {typeof(TState)}");
                return;
            }
            _state = s;
        }

        public virtual TType GetItemType()
        {
            return _type;
        }

        public virtual void SetItemType(TType t)
        {
            _type = t;
        }
        
        public virtual bool IsType<T>(T t)
        {
            if (t is TType state) return _type.Equals(state);
            return false;
        }
        
        public virtual bool IsType<T>(params T[] t)
        {
            return t.Any(IsType);
        }
        
        protected virtual void AnimInX(Vector3 posTarget, float jumpPower, float duration, int nLayer, TState state, bool isCollider = true, bool isScaleItem = true, Vector3 cntRotate = new Vector3())
        {
            StartCoroutine(IEAnimInX(posTarget, jumpPower, duration, nLayer, state, isCollider, isScaleItem, cntRotate));
        }
        protected virtual IEnumerator IEAnimInX(Vector3 posTarget, float jumpPower, float duration, int nLayer, TState state, bool isCollider, bool isScaleItem, Vector3 cntRotate)
        {
            _sortingGroup.sortingOrder = 49;
            _collider.enabled = false;
            TF.DOJump(posTarget, jumpPower, 1, duration);
            if (cntRotate != Vector3.zero)
            {
                TF.DOLocalRotate(TF.localRotation.eulerAngles + cntRotate, duration, RotateMode.FastBeyond360);
            }
            
            yield return WaitForSecondCache.Get(duration);
            
            _sortingGroup.sortingOrder = nLayer;

            if (isScaleItem)
            {
                ScaleItem();
                yield return WaitForSecondCache.Get(_animationClipScale.length + 0.1f);
            }
            
            _collider.enabled = isCollider;
            ChangeState(state);
        }

        protected virtual void ScaleItem()
        {
            if (_animation == null)
            {
                Debug.LogWarning("No animation found");
                return;
            }

            if (_animationClipScale == null)
            {
                Debug.LogWarning("No animation clip found");
                return;
            }

            _animation.Play(_animationClipScale.name);
        }
        
        public virtual void OnClickDown()
        {
            
        }
        
        public virtual void SetMaterial(Material material)
        {
            if (_meshRenderer == null)
            {
                Debug.LogWarning("No mesh renderer found");
                return;
            }
            _meshRenderer.material = material;
        }

        public virtual void SetupItemAlpha()
        {
            if (_itemAlpha == null)
            {
                Debug.LogWarning("No item alpha found");
                return;
            }
            _itemAlpha.Setup();
        }

        public virtual void SetOrder(int nOrder)
        {
            if (_sortingGroup == null)
            {
                Debug.LogWarning("No sorting group found");
                return;
            }
            _sortingGroup.sortingOrder = nOrder;
        }

        public virtual void SetCollider(bool value = true)
        {
            if (_collider == null)
            {
                Debug.LogWarning("No collider found");
                return;
            }
            
            _collider.enabled = value;
        }

        public virtual void SetPosInX(Vector3 posTarget)
        {
            _posInSlot = posTarget;
        }
    }
}