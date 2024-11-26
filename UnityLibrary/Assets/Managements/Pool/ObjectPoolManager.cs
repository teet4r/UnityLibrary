using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectPoolManager : SingletonBehaviour<ObjectPoolManager>
{
    private class ObjectPool
    {
        private Transform _parent;
        private string _prefabName;
        private List<PoolObject> _pool = new();

        public ObjectPool(string prefabName, Transform parent)
        {
            _parent = parent;
            _prefabName = prefabName;
        }

        public PoolObject Get()
        {
            PoolObject pObj = null;

            if (_pool.Count == 0)
            {
                var obj = Addressables.InstantiateAsync(_prefabName, Vector3.zero, Quaternion.identity, _parent).WaitForCompletion();
                obj.TryGetComponent(out pObj);
                return pObj;
            }

            int lastIdx = _pool.Count - 1;
            pObj = _pool[lastIdx];
            _pool.RemoveAt(lastIdx);

            pObj.gameObject.SetActive(true);
            return pObj;
        }

        public void Return(PoolObject obj)
        {
            obj.gameObject.SetActive(false);

            _pool.Add(obj);
        }
    }

    private Transform _tr;
    private Dictionary<Type, ObjectPool> _pools = new();

    protected override void Awake()
    {
        base.Awake();

        _tr = transform;
    }

    public T Get<T>() where T : PoolObject
    {
        return _GetPool<T>().Get() as T;
    }

    public void Return<T>(T obj) where T : PoolObject
    {
        obj.Tr.SetParent(_tr);

        _GetPool(obj.GetType()).Return(obj);
    }

    public void HideAll()
    {
        var allPoolObjects = FindObjectsByType<PoolObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        for (int i = 0; i < allPoolObjects.Length; ++i)
            allPoolObjects[i].Return();
    }

    public void ClearAll()
    {
        var allPoolObjects = FindObjectsByType<PoolObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < allPoolObjects.Length; ++i)
        {
            allPoolObjects[i].Return();
            Addressables.ReleaseInstance(allPoolObjects[i].gameObject);
        }
        _pools.Clear();
    }

    private ObjectPool _GetPool<T>()
    {
        return _GetPool(typeof(T));
    }

    private ObjectPool _GetPool(Type type)
    {
        if (!_pools.TryGetValue(type, out ObjectPool pool))
            _pools.Add(type, pool = new ObjectPool(type.FullName, _tr));

        return pool;
    }
}
