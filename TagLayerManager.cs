using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;

public class TagLayerManager : Singleton<TagLayerManager>
{
    Object[] _asset = null;
    SerializedObject _tagManager = null;
    SerializedProperty _tagProperty = null;
    SerializedProperty _layerProperty = null;

    HashSet<string> _tagSet = new HashSet<string>();
    Dictionary<string, int> _layerDictionary = new Dictionary<string, int>();

    bool _isCreated = false;

    protected override void Awake()
    {
        base.Awake();

        Create();
    }

    public void Create() 
    {
        if (_isCreated) return;
        _isCreated = true;

        _asset = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset");
        if (_asset == null || _asset.Length <= 0)
            throw new System.Exception("There is no TagManager.asset!");
        _tagManager = new SerializedObject(_asset[0]);
        _tagProperty = _tagManager.FindProperty("tags");
        _layerProperty = _tagManager.FindProperty("layers");

        for (int i = 0; i < _tagProperty.arraySize; i++)
            _tagSet.Add(_tagProperty.GetArrayElementAtIndex(i).stringValue);
        for (int i = 0; i < 32; i++)
        {
            var layerName = _layerProperty.GetArrayElementAtIndex(i).stringValue;
            if (string.IsNullOrEmpty(layerName) || string.IsNullOrWhiteSpace(layerName))
                continue;
            _layerDictionary.Add(_layerProperty.GetArrayElementAtIndex(i).stringValue, 1 << i);
        }
    }
    public bool HasTag(string tag)
    {
        return _tagSet.Contains(tag);
    }
    public void AddTags(params string[] newTags)
    {
        for (int i = 0; i < newTags.Length; i++)
        {
            if (HasTag(newTags[i]))
                continue;

            _tagSet.Add(newTags[i]);
            _tagProperty.InsertArrayElementAtIndex(_tagProperty.arraySize - 1);
            _tagProperty.GetArrayElementAtIndex(_tagProperty.arraySize - 1).stringValue = newTags[i];
            _tagManager.ApplyModifiedProperties();
            _tagManager.Update();
        }
    }
    public int GetLayers(params string[] layerNames)
    {
        int layerMasks = 0;
        for (int i = 0; i < layerNames.Length; i++)
            if (_layerDictionary.TryGetValue(layerNames[i], out int layerMask))
                layerMasks |= layerMask;
        return layerMasks;
    }
    public void AddLayers(params string[] newLayerNames)
    {
        for (int i = 0; i < newLayerNames.Length; i++)
        {
            if (GetLayers(newLayerNames[i]) != 0)
                continue;
            
            for (int j = 8; j < 32; j++)
            {
                var element = _layerProperty.GetArrayElementAtIndex(j);
                if (string.IsNullOrEmpty(element.stringValue) || string.IsNullOrWhiteSpace(element.stringValue))
                {
                    _layerDictionary.Add(newLayerNames[i], j);
                    _layerProperty.GetArrayElementAtIndex(j).stringValue = newLayerNames[i];
                    _tagManager.ApplyModifiedProperties();
                    _tagManager.Update();
                    break;
                }
            }
        }
    }
}