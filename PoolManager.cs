using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    class ObjectPool : MonoBehaviour
    {
        public GameObject prefab;

        Queue<GameObject> _q = new Queue<GameObject>();

        public GameObject Get()
        {
            if (_q.Count == 0)
            {
                var clone = Instantiate(prefab, transform);
                clone.name = prefab.name;
                return clone;
            }
            return _q.Dequeue();
        }
        public void Put(GameObject obj)
        {
            if (obj == null)
            {
                Debug.LogError("This object is null.");
                return;
            }
            if (!obj.activeSelf) return;

            obj.SetActive(false);
            _q.Enqueue(obj);
        }
    }

    ObjectPool _getPoolObj = null;
    ObjectPool _putPoolObj = null;
    Dictionary<string, ObjectPool> _poolDictionary = new Dictionary<string, ObjectPool>();

    public GameObject Get(string prefabName)
    {
        var prefab = ResourceManager.Instance.Get<GameObject>(prefabName);
        if (prefab == null)
        {
            Debug.LogError($"{prefabName} doesn't exist!");
            return null;
        }
        
        var poolName = $"{prefabName}Pool";
        if (_poolDictionary.TryGetValue(poolName, out _getPoolObj))
            return _getPoolObj.Get();

        var newPoolObj = new GameObject(poolName);
        var pool = newPoolObj.AddComponent<ObjectPool>();

        pool.transform.parent = transform;
        pool.prefab = prefab;
        _poolDictionary.Add(poolName, pool);
        return pool.Get();
    }
    public void Put(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogError("Null GameObject is detected!");
            return;
        }

        var poolName = $"{obj.name}Pool";
        if (_poolDictionary.TryGetValue(poolName, out _putPoolObj))
        {
            _putPoolObj.Put(obj);
            return;
        }
        Debug.LogError("Null pool is detected!");
    }
}