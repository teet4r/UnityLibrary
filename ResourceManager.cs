using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ResourceManager : Singleton<ResourceManager>
{
    class TypePool : MonoBehaviour
    {
        Object _obj = null;
        Dictionary<string, Object> _dictionary = new Dictionary<string, Object>();

        public Object Get(string resourceName)
        {
            if (_dictionary.TryGetValue(resourceName, out _obj))
                return _obj;
            return null;
        }
        public void Add(string resourceName, Object resource)
        {
            _dictionary.Add(resourceName, resource);
        }
    }

    bool _isLoaded = false;
    TypePool _pool = null;
    Dictionary<string, TypePool> _typeDictionary = new Dictionary<string, TypePool>();

    protected override void Awake()
    {
        base.Awake();

        LoadAll();
    }

    public void LoadAll()
    {
        if (_isLoaded) return;
        _isLoaded = true;

        // 해당 경로 리소스 모두 로드
        var resources = Resources.LoadAll("");
        for (int i = 0; i < resources.Length; i++)
        {
            var type = resources[i].GetType();
            var typeName = type.Name;

            // 해당 타입이 없으면 풀 추가
            if (!_typeDictionary.ContainsKey(typeName))
            {
                var newObj = new GameObject($"{typeName}Resources");
                var pool = newObj.AddComponent<TypePool>();
                pool.transform.parent = transform;
                _typeDictionary.Add(typeName, pool);
            }

            // 게임오브젝트인 경우 원본 형태를 유지하기 위해 복제본 생성 후 비활성화하여 저장
            if (type == typeof(GameObject))
            {
                var resource = resources[i] as GameObject;
                var originActiveSelf = resource.activeSelf;

                if (originActiveSelf)
                    resource.SetActive(false);

                var clone = Instantiate(resource, transform);
                clone.transform.parent = _typeDictionary[typeName].transform;
                clone.name = resource.name;
                resources[i] = clone;

                if (originActiveSelf)
                    resource.SetActive(true);
            }

            _typeDictionary[typeName].Add(resources[i].name, resources[i]);
        }
    }
    public T Get<T>(string resourceName) where T : Object
    {
        var typeName = typeof(T).Name;
        if (_typeDictionary.TryGetValue(typeName, out _pool))
            return _pool.Get(resourceName) as T;
        Debug.LogError($"{typeName} doesn't exist!");
        return null;
    }
}