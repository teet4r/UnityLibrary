using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameObject를 저장하는 풀
/// Prefab 형태는 모두 여기서 관리
/// </summary>
public class PoolManager : Singleton<PoolManager>
{
    class ObjectPool : MonoBehaviour
    {
        public GameObject prefab;

        Queue<GameObject> _q = new Queue<GameObject>();

        void Start()
        {
            if (prefab.GetComponent<AutoReturnToPool>() == null)
                throw new System.Exception($"{prefab.name} prefab has no AutoReturnToPool component!");
        }

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

            // 이미 비활성화되어 있다면 큐에 넣지 않음
            if (!obj.activeSelf) return;

            obj.SetActive(false);
            _q.Enqueue(obj);
        }
    }

    ObjectPool _getPoolObj = null;
    ObjectPool _putPoolObj = null;
    Dictionary<string, ObjectPool> _poolDictionary = new Dictionary<string, ObjectPool>();
    bool _isCreated = false;

    void Awake()
    {
        Create();
    }

    public void Create()
    {
        if (_isCreated) return;
        _isCreated = true;
    }
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

        // 새로 생성된 풀은 풀 매니저 하위 오브젝트로 등록
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