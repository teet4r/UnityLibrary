using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceManager : Singleton<ResourceManager>
{
    class ResourcePool : MonoBehaviour
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
    ResourcePool _pool = null;
    Dictionary<string, ResourcePool> _resourceDictionary = new Dictionary<string, ResourcePool>();

    protected override void Awake()
    {
        base.Awake();

        LoadAll();
    }

    public void LoadAll()
    {
        if (_isLoaded) return;
        _isLoaded = true;

        var resources = Resources.LoadAll("");
        for (int i = 0; i < resources.Length; i++)
        {
            var type = resources[i].GetType();
            var typeName = type.Name;
            if (!_resourceDictionary.ContainsKey(typeName))
            {
                var newObj = new GameObject($"{typeName}Resources");
                var pool = newObj.AddComponent<ResourcePool>();
                pool.transform.parent = transform;
                _resourceDictionary.Add(typeName, pool);
            }
            _resourceDictionary[typeName].Add(resources[i].name, resources[i]);
        }
    }
    public T Get<T>(string resourceName) where T : Object
    {
        var typeName = typeof(T).Name;
        if (_resourceDictionary.TryGetValue(typeName, out _pool))
            return _pool.Get(resourceName) as T;
        Debug.LogError($"{typeName} doesn't exist!");
        return null;
    }
}
//// for keeping its original state.
//var originActiveSelf = resources[j].activeSelf;

//if (originActiveSelf)
//    resources[j].SetActive(false);

//var clone = Instantiate(prefabs[j], transform);
//clone.name = prefabs[j].name;
//_prefabDictionary.Add(clone.name, clone);

//if (originActiveSelf)
//    prefabs[i].SetActive(true);