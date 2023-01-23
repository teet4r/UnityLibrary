using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    [SerializeField] string[] _paths =
    {
        "Prefabs", "Sounds"
    };

    bool _isLoaded = false;
    GameObject _getObj = null;
    Dictionary<string, GameObject> _prefabDictionary = new Dictionary<string, GameObject>();

    protected override void Awake()
    {
        base.Awake();

        LoadAll();
    }

    /// <summary>
    /// Use a clone of original prefab.
    /// </summary>
    public void LoadAll()
    {
        if (_isLoaded) return;
        _isLoaded = true;

        for (int i = 0; i < _paths.Length; i++)
        {
            var prefabs = Resources.LoadAll<GameObject>(_paths[i]);
            if (prefabs == null)
                Debug.LogError($"There are no prefabs in {_paths[i]}!");

            for (int j = 0; j < prefabs.Length; j++)
            {
                // for keeping its original state.
                var originActiveSelf = prefabs[j].activeSelf;

                if (originActiveSelf)
                    prefabs[j].SetActive(false);

                var clone = Instantiate(prefabs[j], transform);
                clone.name = prefabs[j].name;
                _prefabDictionary.Add(clone.name, clone);

                if (originActiveSelf)
                    prefabs[i].SetActive(true);
            }
        }
    }
    public GameObject Get(string prefabName)
    {
        if (_prefabDictionary.TryGetValue(prefabName, out _getObj))
            return _getObj;
        return null;
    }
}
