using System;
using System.Threading;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public new Transform transform => _transform;
    private Transform _transform;

    public event Action onReturn;

    private bool IsTokenCancellable => !_cancellationTokenSource.IsNull() && !_cancellationTokenSource.IsCancellationRequested;
    protected CancellationTokenSource CancellationTokenSource
    {
        get
        {
            if (!IsTokenCancellable)
                _cancellationTokenSource = new CancellationTokenSource();
            return _cancellationTokenSource;
        }
    }
    private CancellationTokenSource _cancellationTokenSource;

    protected virtual void Awake()
    {
        _transform = gameObject.transform;

        ObjectPoolManager.Instance.onHideOrClear += Return;
    }

    protected virtual void OnDestroy()
    {
        ObjectPoolManager.Instance.onHideOrClear -= Return;
    }

    public void Return()
    {
        if (!gameObject.activeSelf)
            return;

        gameObject.SetActive(false);
        onReturn?.Invoke();
        if (IsTokenCancellable)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
        ObjectPoolManager.Instance.Return(this);
    }
}