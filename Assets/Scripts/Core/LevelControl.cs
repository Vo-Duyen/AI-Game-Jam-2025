using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DesignPattern;
using DesignPattern.ObjectPool;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LongNC
{
    public class LevelControl : Singleton<LevelControl>
    {
        [Title("Load level")]
        [SerializeField, Required] private LevelData _levelData;
        [SerializeField, Required] private GameObject _blockPrefab;
#if UNITY_EDITOR
        [ShowInInspector] private Object _folderBlockMaterials;
#endif
        [SerializeField, Required] private List<Material> _blockMaterials = new List<Material>();
        
        [Title("Gameplay")]
        [SerializeField] private bool _isControl;
        [SerializeField] private int _maxSlots = 5;
        [SerializeField] private Vector3 _slotStartPosition = new Vector3(-4f, 0f, 0f);
        [SerializeField] private float _distanceBetweenSlot = 2f;
        [ShowInInspector, ReadOnly] private List<(Transform pos, ItemIdle value)> _arrPosSlot = new List<(Transform pos, ItemIdle value)>();
        [ShowInInspector, ReadOnly] private int _curIdPosSlot = 0;
        [ShowInInspector, ReadOnly] private Dictionary<Enum, Stack<ItemIdle>> _arrSlot = new Dictionary<Enum, Stack<ItemIdle>>();
        [SerializeField] private float _distanceBetweenBlock = 0.2f;
        [ShowInInspector, ReadOnly] private List<Stack<ItemIdle>> _arrPile = new List<Stack<ItemIdle>>();

        private Dictionary<int, Transform> _checkLoadLevel = new Dictionary<int, Transform>();
        private Dictionary<Transform, int> _getIdArrPile = new Dictionary<Transform, int>();
        private ItemIdle _itemTarget;
        private Transform _slotContainer;
        private int _cntBlock;
        
#if UNITY_EDITOR        
        [Button]
        public void LoadBlockMaterials()
        {
            var folderPath = AssetDatabase.GetAssetPath(_folderBlockMaterials);

            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                Debug.LogError($"{folderPath} is not a valid folder");
                return;
            }
            
            var guids = AssetDatabase.FindAssets("t:Material", new[] { folderPath });

            if (guids.Length == 0)
            {
                Debug.LogError($"{folderPath} is not a valid folder");
                return;
            }

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var material = AssetDatabase.LoadAssetAtPath<Material>(path);

                if (material != null)
                {
                    _blockMaterials.Add(material);
                }
            }
            
            Debug.Log($"{folderPath} has {_blockMaterials.Count} materials");
        }
#endif        
        
        [Button]
        public void LoadLevel(int level)
        {
            var dataLevel = Resources.Load<LevelData>($"Data/DataLevel_{level}");
            if (dataLevel == null)
            {
                Debug.LogError($"Level {level} not found");
                return;
            }
            _levelData = dataLevel;
            Debug.Log($"Level {level} loaded");
        }

        [Button]
        public void LoadAllObjectInLevel()
        {
            if (_levelData == null)
            {
                Debug.LogError("Data level not found");
                return;
            }

            var curLevel = _levelData.level;
            var pileStructures = _levelData.pileStructures;
            var idPileStructure = 0;

            if (!_checkLoadLevel.ContainsKey(curLevel))
            {
                _checkLoadLevel[curLevel] = new GameObject(name: $"Level_{curLevel}").transform;
            }

            // LoadSlot
            _arrSlot.Clear();
            LoadSlots(curLevel);
            
            // LoadBlock
            _arrPile = Enumerable.Range(0, pileStructures.Count).Select(_ => new Stack<ItemIdle>()).ToList();
            foreach (var pileStructure in pileStructures)
            {
                var curTransformPileStructures = new GameObject(name: $"Pile_{idPileStructure}").transform;
                curTransformPileStructures.SetParent(_checkLoadLevel[curLevel]);
                curTransformPileStructures.position = pileStructure.defaultPosition;

                var curOrder = 0;
                
                foreach (var blockType in pileStructure.blocks)
                {
                    ++_cntBlock;
                    var block = PoolingManager.Spawn(_blockPrefab, 
                        curTransformPileStructures.position + Vector3.up * _distanceBetweenBlock * _arrPile[idPileStructure].Count, 
                        Quaternion.Euler(-45, 0, 0), 
                        curTransformPileStructures).transform;
                    
                    var itemIdle = block.GetComponent<ItemIdle>();
                    itemIdle.SetMaterial(new Material(_blockMaterials[blockType.colorId]));
                    itemIdle.SetItemType((ColorId) blockType.colorId);
                    itemIdle.SetupItemAlpha();
                    itemIdle.SetOrder(curOrder++);
                    
                    if (curOrder != pileStructure.blocks.Count)
                    {
                        itemIdle.SetCollider(false);
                    }

                    _arrPile[idPileStructure].Push(itemIdle);
                    _getIdArrPile[block] = idPileStructure;
                }
                
                ++idPileStructure;
            }
        }

        private void LoadSlots(int curLevel)
        {
            if (_slotContainer == null)
            {
                _slotContainer = new GameObject("SlotContainer").transform;
                _slotContainer.SetParent(_checkLoadLevel[curLevel]);
            }

            _arrPosSlot.Clear();

            for (var i = 0; i < _maxSlots; i++)
            {
                var slotTransform = new GameObject($"Slot_{i + 1}").transform;
                slotTransform.SetParent(_slotContainer);
                slotTransform.position = _slotStartPosition + Vector3.right * (_distanceBetweenSlot * i);
                
                _arrPosSlot.Add((slotTransform, null));
            }
        }

        private void Update()
        {
            if (_isControl)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _itemTarget = GetItemIdle();
                    if (_itemTarget != null)
                    {
                        if (GetPosBlockToSlot(_itemTarget))
                        {
                            _itemTarget.OnClickDown();
                            RemoveBlockInArrPile(_getIdArrPile[_itemTarget.TF]);
                            SetControl(false);
                        }
                        else
                        {
                            // TODO: Lose
                        }
                    }
                }
            }
        }

        public ItemIdle GetItemIdle()
        {
            ItemIdle itemIdle = null;
            ItemIdle itemCheck = null;
            
            if (Camera.main != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit[] hits = Physics.RaycastAll(ray, Mathf.Infinity);
                
                foreach (var hit in hits)
                {
                    itemCheck = hit.transform.GetComponent<ItemIdle>();
                    if (itemCheck != null && 
                        (itemIdle == null || itemIdle.Order < itemCheck.Order))
                    {
                        itemIdle = itemCheck;
                    }
                }
            }
            return itemIdle;
        }

        public bool GetPosBlockToSlot(ItemIdle block)
        {
            if (_arrPosSlot[_maxSlots - 1].value != null)
            {
                return false;
            }

            var posInSlot = Vector3.zero;
            _arrSlot.TryAdd(block.GetItemType(), new Stack<ItemIdle>());
            switch (_arrSlot[block.GetItemType()].Count)
            {
                case 0:
                    posInSlot = _arrPosSlot[_curIdPosSlot].pos.position;
                    _arrSlot[block.GetItemType()].Push(block);
                    _arrPosSlot[_curIdPosSlot] = (_arrPosSlot[_curIdPosSlot].pos, block);
                    ++_curIdPosSlot;
                    SetControl(true, 0.5f);
                    break;
                case 1:
                    var lastItem = _arrSlot[block.GetItemType()].Peek();
                    var nextId = 0;
                    foreach (var child in _arrPosSlot)
                    {
                        ++ nextId;
                        if (child.value == lastItem)
                        {
                            break;
                        }
                    }
                    posInSlot = _arrPosSlot[nextId].pos.position;
                    _arrSlot[block.GetItemType()].Push(block);

                    MoveAllBlockToRight(nextId);
                    _arrPosSlot[nextId] = (_arrPosSlot[nextId].pos, block);
                    
                    ++_curIdPosSlot;
                    SetControl(true, 0.5f);
                    break;
                case 2:
                    lastItem = _arrSlot[block.GetItemType()].Peek();
                    nextId = 0;
                    foreach (var child in _arrPosSlot)
                    {
                        ++ nextId;
                        if (child.value == lastItem)
                        {
                            break;
                        }
                    }
                    posInSlot = _arrPosSlot[nextId].pos.position;
                    _arrSlot[block.GetItemType()].Push(block);

                    MoveAllBlockToRight(nextId);
                    _arrPosSlot[nextId] = (_arrPosSlot[nextId].pos, block);

                    StartCoroutine(IERemove3Block(block, 0.5f, nextId));
                    
                    _curIdPosSlot -= 2;
                    break;
            }
            
            block.SetPosInX(posInSlot);
                
            return true;
        }

        private IEnumerator IERemove3Block(ItemIdle block, float timeDelay, int id)
        {
            yield return WaitForSecondCache.Get(timeDelay);
            
            while (_arrSlot[block.GetItemType()].Count > 0)
            {
                var curItemIdle = _arrSlot[block.GetItemType()].Pop();
                curItemIdle.ChangeState(ItemIdle.State.AnimDone);
            }

            for (var i = 0; i < 3; ++i)
            {
                _arrPosSlot[id - i] = (_arrPosSlot[id - i].pos, null);
            }
            
            // MoveBlock
            MoveAllBlockToLeft(id, 3);
        }
        
        public void RemoveBlockInArrPile(int id)
        {
            _arrPile[id].Pop();
            if (_arrPile[id].Count != 0)
            {
                _arrPile[id].Peek().SetCollider();
            }
        }

        public void MoveAllBlockToLeft(int id, int distanceId = 1)
        {
            for (var i = id; i < _maxSlots; ++i)
            {
                MoveBlockToLeft(i, distanceId);
            }
        }
        
        public void MoveBlockToLeft(int curIdInSlot, int distanceId = 1)
        {
            if (_arrPosSlot[curIdInSlot].value == null)
            {
                return;
            }
            var block = _arrPosSlot[curIdInSlot].value;
            block.TF.DOJump(_arrPosSlot[curIdInSlot - distanceId].pos.position, 0.5f, 1, 0.5f);
            _arrPosSlot[curIdInSlot] = (_arrPosSlot[curIdInSlot].pos, null);
            _arrPosSlot[curIdInSlot - distanceId] = (_arrPosSlot[curIdInSlot - distanceId].pos, block);
        }
        
        public void MoveAllBlockToRight(int id)
        {
            for (var i = _maxSlots - 1; i >= id; -- i)
            {
                MoveBlockToRight(i);
            }
        }

        public void MoveBlockToRight(int curIdInSlot)
        {
            if (_arrPosSlot[curIdInSlot].value == null)
            {
                return;
            }
            var block = _arrPosSlot[curIdInSlot].value;
            block.TF.DOJump(_arrPosSlot[curIdInSlot + 1].pos.position, 0.5f, 1, 0.5f);
            _arrPosSlot[curIdInSlot + 1] = (_arrPosSlot[curIdInSlot + 1].pos, block);
        }

        public void SetControl(bool value, float timeDelay = 0f)
        {
            if (timeDelay == 0)
            {
                _isControl = value;
                return;
            }
            DOVirtual.DelayedCall(timeDelay, () =>
            {
                _isControl = value;
            });
        }

        public void CheckLevel()
        {
            --_cntBlock;
            if (_cntBlock == 0)
            {
                Debug.Log($"[Complete] Win level {_levelData.level}!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                SetControl(false);
            }
        }
    }
}