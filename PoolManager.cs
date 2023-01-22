using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.AI;

public class PoolManager : MonoBehaviour
{
    class ObjectPool : MonoBehaviour
    {
        public GameObject prefab;

        Queue<GameObject> _q = new Queue<GameObject>();

        void OnDestroy()
        {
            Clear();
        }

        public GameObject Get()
        {
            if (_q.Count == 0)
            {
                var clone = Instantiate(prefab);
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
        public void Clear()
        {
            while (_q.Count != 0)
                Destroy(_q.Dequeue());
        }
    }

    public static PoolManager instance = null;

    Dictionary<string, GameObject> _prefabDictionary = new Dictionary<string, GameObject>();
    Dictionary<string, ObjectPool> _poolDictionary = new Dictionary<string, ObjectPool>();

    void Awake()
    {
        if (instance == null)
            instance = this;

        Initialize();
    }
    void OnDestroy()
    {
        Clear();
    }

    public GameObject Get(string prefabName)
    {
        if (!_prefabDictionary.ContainsKey(prefabName))
        {
            Debug.LogError($"{prefabName} doesn't exist in Resources/Prefabs folder.");
            return null;
        }

        var poolName = $"{prefabName}Pool";
        if (!_poolDictionary.ContainsKey(poolName))
        {
            var poolObj = new GameObject(poolName);
            var pool = poolObj.AddComponent<ObjectPool>();

            pool.transform.parent = transform;
            pool.prefab = _prefabDictionary[prefabName];
            _poolDictionary.Add(poolName, pool);
        }
        return _poolDictionary[poolName].Get();
    }
    public void Put(GameObject obj)
    {
        if (obj == null) return;

        var poolName = $"{obj.name}Pool";
        if (!_poolDictionary.ContainsKey(poolName)) return;

        _poolDictionary[poolName].Put(obj);
    }
    public void Clear()
    {
        foreach (var pair in _poolDictionary)
            Destroy(pair.Value);
    }

    void Initialize()
    {
        var prefabs = Resources.LoadAll<GameObject>("Prefabs");
        if (prefabs == null)
            Debug.LogError("There are no prefabs!");

        for (int i = 0; i < prefabs.Length; i++)
        {
            prefabs[i].SetActive(false);
            _prefabDictionary.Add(prefabs[i].name, prefabs[i]);
        }
    }
}