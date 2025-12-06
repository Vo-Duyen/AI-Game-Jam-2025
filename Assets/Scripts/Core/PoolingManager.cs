using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace DesignPattern.ObjectPool
{
    public static class PoolingManager
    {
        private const int DefaultPoolSize = 3;
        private static Dictionary<int, Pool> _pools;

        private static void Init(GameObject prefab = null, int initQuantity = DefaultPoolSize)
        {
            _pools ??= new Dictionary<int, Pool>();
            if (!prefab) return;
            int idObject = prefab.GetInstanceID();
            if (!_pools.ContainsKey(idObject))
            {
                _pools[idObject] = new Pool(prefab, initQuantity);
            }
        }

        public static void PoolPreload(GameObject prefab, int quantity, Transform parent = null)
        {
            Init(prefab, 1);
            _pools[prefab.GetInstanceID()].Preload(quantity, parent);
        }

        public static GameObject[] Preload(GameObject prefab, int quantity = 1, Transform parent = null)
        {
            Init(prefab, quantity);
            var gameObjects = new GameObject[quantity];
            for (int i = 0; i < quantity; ++i)
            {
                gameObjects[i] = Spawn(prefab, Vector3.zero, Quaternion.identity);
                if (parent != null)
                {
                    gameObjects[i].transform.SetParent(parent);
                }
            }
            return gameObjects;
        }
        
        public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion quaternion, Transform parent = null)
        {
            Init(prefab);
            return _pools[prefab.GetInstanceID()].Spawn(position, quaternion, parent);
        }

        public static GameObject Spawn(GameObject prefab)
        {
            return Spawn(prefab, Vector3.zero, Quaternion.identity);
        }
        
        public static T Spawn<T>(T prefab) where T : Component
        {
            return Spawn(prefab, Vector3.zero, Quaternion.identity);
        }

        public static T Spawn<T>(T prefab, Vector3 positon, Quaternion quaternion) where T : Component
        {
            Init(prefab.gameObject);
            return _pools[prefab.GetInstanceID()].Spawn<T>(positon, quaternion);
        }

        public static void Despawn(GameObject gameObject, UnityAction callback = null)
        {
            Pool p = _pools.Values.FirstOrDefault(pool => pool.ObjectIDs.Contains(gameObject.GetInstanceID()));

            if (p != null)
            {
                callback?.Invoke();
                p.Despawn(gameObject);
            }
            else
            {
                Object.Destroy(gameObject);
            }
        }
        
        public static int GetStackCount(GameObject prefab)
        {
            _pools ??= new Dictionary<int, Pool>();
            if (prefab == null) return 0;
            return _pools.ContainsKey(prefab.GetInstanceID()) ? _pools[prefab.GetInstanceID()].DeActiveGameObjectCount : 0;
        }
    }

    public class Pool
    {
        private int _id = 1;
        private readonly Queue<GameObject> _deActiveQueue;
        public readonly HashSet<int> ObjectIDs;
        private readonly GameObject _prefab;

        public int DeActiveGameObjectCount => _deActiveQueue.Count;
        
        public Pool(GameObject prefab, int initQuantity)
        {
            _prefab = prefab;
            _deActiveQueue = new Queue<GameObject>(initQuantity);
            ObjectIDs = new HashSet<int>();
        }

        public void Preload(int initQuantity, Transform parent = null)
        {
            for (int i = 0; i < initQuantity; ++i)
            {
                var gameObject = Object.Instantiate(_prefab, parent);
                gameObject.name = _prefab.name + "_" + _id ++;
                ObjectIDs.Add(gameObject.GetInstanceID());
                gameObject.SetActive(false);
                _deActiveQueue.Enqueue(gameObject);
            }
        }

        public GameObject Spawn(Vector3 position, Quaternion quaternion, Transform parent = null)
        {
            while (true)
            {
                GameObject newObject;
                if (_deActiveQueue.Count == 0)
                {
                    newObject = Object.Instantiate(_prefab);
                    newObject.name = _prefab.name + "_" + _id ++;
                    ObjectIDs.Add(newObject.GetInstanceID());
                }
                else
                {
                    newObject = _deActiveQueue.Dequeue();
                    if (!newObject)
                    {
                        continue;
                    }
                }
                newObject.SetActive(true);
                newObject.transform.SetPositionAndRotation(position,quaternion);
                if (parent != null) newObject.transform.SetParent(parent);
                return newObject;
            }
        }

        public T Spawn<T>(Vector3 position, Quaternion quaternion)
        {
            return Spawn(position, quaternion).GetComponent<T>();
        }

        public void Despawn(GameObject gameObject)
        {
            if (!gameObject.activeSelf)
            {
                return;
            }
            gameObject.SetActive(false);
            _deActiveQueue.Enqueue(gameObject);
        }
    }
}