using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Resources 폴더 활용
/// 모든 Resource 폴더를 검사하기 때문에
/// 형식에 상관없이 Resources 폴더 하위에 넣어두면 됨.
/// </summary>
public class ResourceManager : Singleton<ResourceManager>
{
    class TypePool : MonoBehaviour
    {
        Dictionary<string, Object> _dictionary = new Dictionary<string, Object>();

        public Object Get(string resourceName)
        {
            if (_dictionary.TryGetValue(resourceName, out Object _obj))
                return _obj;
            return null;
        }
        public void Add(string resourceName, Object resource)
        {
            if (!_dictionary.TryGetValue(resourceName, out Object _obj))
                _dictionary.Add(resourceName, resource);
        }
    }

    bool _isCreated = false;
    TypePool _pool = null;
    Dictionary<string, TypePool> _typeDictionary = new Dictionary<string, TypePool>();

    protected override void Awake()
    {
        base.Awake();

        Create();
    }

    public void Create()
    {
        if (_isCreated) return;
        _isCreated = true;

        // Resource 파일 모두 로드
        var resources = Resources.LoadAll("");
        for (int i = 0; i < resources.Length; i++)
        {
            var type = resources[i].GetType();
            var typeName = type.Name;

            // 해당 타입의 풀이 없으면 추가
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

                // 비활성화 후 복제
                if (originActiveSelf)
                    resource.SetActive(false);

                var clone = Instantiate(resource, transform);
                clone.transform.parent = _typeDictionary[typeName].transform;
                clone.name = resource.name;
                resources[i] = clone;

                // 원래 상태로 되돌림
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