using System;
using System.Collections.Generic;
using UniRx;
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

        public void Clear()
        {
            foreach (var obj in _pool)
                Addressables.ReleaseInstance(obj.gameObject);
            _pool.Clear();
        }
    }

    private Transform _tr;
    private Dictionary<Type, ObjectPool> _pools = new();
    public IObservable<bool> OnHideOrClear => _onHideOrClearSubject;
    private Subject<bool> _onHideOrClearSubject = new();

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
        _onHideOrClearSubject.OnNext(true);
    }

    public void ClearAll()
    {
        HideAll();
        foreach (var pool in _pools.Values)
            pool.Clear();
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
