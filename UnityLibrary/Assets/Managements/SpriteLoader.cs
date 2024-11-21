using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SpriteLoader : SingletonBehaviour<SpriteLoader>
{
    private Dictionary<string, Sprite> _dict = new();

    public Sprite Load(string sourceName)
    {
        if (!_dict.TryGetValue(sourceName, out Sprite sprite))
        {
            var obj = Addressables.LoadAssetAsync<Sprite>(sourceName).WaitForCompletion();
            if (obj == null)
                return null;

            _dict.Add(sourceName, sprite = obj);
            Addressables.Release(obj);
        }
        return sprite;
    }

    public void Clear()
    {
        _dict.Clear();
    }
}
