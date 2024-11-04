using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : SingletonBehaviour<ObjectPoolManager>
{
    private class ObjectPool
    {
        private Transform _parent;
        private PoolObject _prefab; // 풀에 재사용할 오브젝트는 PoolObject를 상속해야 함
        private PoolObject[] _pool = new PoolObject[1];
        private int _top = -1;

        public ObjectPool(string prefabName, Transform parent)
        {
            _parent = parent;
            _prefab = Resources.Load<PoolObject>($"Prefabs/{prefabName}");
            //var prefab = Addressables.LoadAssetAsync<GameObject>(prefabName);
            //prefab.WaitForCompletion();
            //_prefab = prefab.Result.GetComponent<PoolObject>(); // 스크립트와 프리팹 이름은 동일하게
            //Addressables.Release(prefab);
        }

        public PoolObject Get()
        {
            if (_top < 0)
                return Instantiate(_prefab, Vector3.positiveInfinity, Quaternion.identity, _parent);

            var obj = _pool[_top--];
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(PoolObject obj)
        {
            obj.gameObject.SetActive(false);

            ++_top;

            if (_top >= _pool.Length)
                _ResizePool();

            _pool[_top] = obj;
        }

        public void Clear()
        {
            while (_top >= 0)
            {
                var obj = _pool[_top];

                Destroy(obj.gameObject);
                _pool[_top] = null;
                --_top;
            }
        }

        private void _ResizePool()
        {
            int poolSize = _pool.Length;
            PoolObject[] newPool = new PoolObject[_pool.Length << 1];

            for (int i = 0; i < poolSize; ++i)
                newPool[i] = _pool[i];

            _pool = newPool;
        }
    }

    private Transform _tr;
    private Dictionary<Type, ObjectPool> _pools = new();

    protected override void Awake()
    {
        base.Awake();

        _tr = GetComponent<Transform>();
    }

    public T Get<T>() where T : PoolObject =>
        _GetPool<T>().Get() as T;

    public void Return<T>(T obj) where T : PoolObject
    {
        if (obj.Tr.parent != _tr)
            obj.Tr.SetParent(_tr);

        _GetPool<T>().Return(obj);
    }

    public void HideAll()
    {
        var children = _tr.GetComponentsInChildren<PoolObject>(false);

        for (int i = 0; i < children.Length; ++i)
            Return(children[i]);
    }

    public void ClearAll()
    {
        foreach (var pool in _pools.Values)
            pool.Clear();
        _pools.Clear();
    }

    private ObjectPool _GetPool<T>()
    {
        var type = typeof(T);

        if (!_pools.TryGetValue(type, out ObjectPool pool))
            _pools.Add(type, pool = new ObjectPool(type.Name, _tr));

        return pool;
    }
}
