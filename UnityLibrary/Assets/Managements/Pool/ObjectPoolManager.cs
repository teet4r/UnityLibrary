using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ObjectPoolManager : SingletonBehaviour<ObjectPoolManager>
{
    public new Transform transform => _transform;
    private Transform _transform;

    public event Action onHideOrClear;

    private Dictionary<string, List<PoolObject>> _pools = new();

    protected override void Awake()
    {
        base.Awake();

        _transform = gameObject.transform;
    }

    public PoolObject Get(string prefabName)
    {
        PoolObject pObj = null;

        if (!_pools.TryGetValue(prefabName, out List<PoolObject> pool))
        {
            var obj = Addressables.InstantiateAsync(prefabName, new Vector2(9999f, 9999f), Quaternion.identity, transform).WaitForCompletion();

            if (obj.TryGetComponent(out pObj))
            {
                obj.name = prefabName;
                _pools.Add(prefabName, pool = new List<PoolObject>());
            }

            return pObj;
        }

        if (pool.Count == 0)
        {
            var obj = Addressables.InstantiateAsync(prefabName, new Vector2(9999f, 9999f), Quaternion.identity, transform).WaitForCompletion();

            if (obj.TryGetComponent(out pObj))
                obj.name = prefabName;

            return pObj;
        }

        int lastIdx = pool.Count - 1;
        pObj = pool[lastIdx];
        pool.RemoveAt(lastIdx);

        pObj.gameObject.SetActive(true);

        return pObj;
    }

    public void Return(PoolObject obj)
    {
        if (!_pools.TryGetValue(obj.name, out List<PoolObject> pool))
            _pools.Add(obj.name, pool = new List<PoolObject>());
        pool.Add(obj);
    }

    public void HideAll()
    {
        onHideOrClear?.Invoke();
    }

    public void ClearAll()
    {
        HideAll();
        foreach (var pool in _pools.Values)
        {
            for (int i = 0; i < pool.Count; ++i)
                Addressables.ReleaseInstance(pool[i].gameObject);
            pool.Clear();
        }
        _pools.Clear();
    }
}
