using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class UIManager : SingletonBehaviour<UIManager>
{
    private RectTransform _rectTr;
    private CanvasScaler _canvasScaler;
    [SerializeField] private Transform _active;
    [SerializeField] private Transform _inactive;

    private Dictionary<Type, UI> _uiPool = new();

    private float _heightRatio;
    private float _widthRatio;

    protected override void Awake()
    {
        base.Awake();

        _rectTr = (RectTransform)transform;
        TryGetComponent(out _canvasScaler);

        _heightRatio = Screen.height / _canvasScaler.referenceResolution.y;
        _widthRatio = Screen.width / _canvasScaler.referenceResolution.x;
    }

    public T Show<T>() where T : UI
    {
        var type = typeof(T);

        if (!_uiPool.TryGetValue(type, out UI ui))
        {
            var obj = Addressables.InstantiateAsync(type.FullName, _active).WaitForCompletion();
            if (obj.TryGetComponent(out UI newUI))
                _uiPool.Add(type, newUI);
            return newUI as T;
        }

        var t = ui as T;
        t.RectTr.SetParent(_active);
        t.gameObject.SetActive(true);
        return t;
    }

    public void Hide(UI target)
    {
        if (!_uiPool.TryGetValue(target.GetType(), out UI ui))
            return;

        ui.RectTr.SetParent(_inactive);
        ui.gameObject.SetActive(false);
    }

    public void HideAll()
    {
        foreach (var ui in _uiPool.Values)
            Hide(ui);
    }

    public void ClearAll()
    {
        foreach (var ui in _uiPool.Values)
            Addressables.Release(ui.gameObject);
        _uiPool.Clear();
    }

    /// <summary>
    /// 절대 길이를 상대 길이로 변환
    /// </summary>
    /// <param name="height"></param>
    /// <returns></returns>
    public float ToReactiveHeight(float height)
    {
        return height * _heightRatio;
    }

    public float ToReactiveWidth(float width)
    {
        return width * _widthRatio;
    }

    public Vector2 ToReactiveSize(Vector2 size)
    {
        size.y = ToReactiveHeight(size.y);
        size.x = ToReactiveWidth(size.x);

        return size;
    }
}
