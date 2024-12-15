using System.Threading;
using UniRx;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public new Transform transform => _transform;
    private Transform _transform;

    protected bool IsTokenCancellable => !_cancellationTokenSource.IsNull() && !_cancellationTokenSource.IsCancellationRequested;
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

        ObjectPoolManager.Instance.OnHideOrClear.Subscribe(_ => Return())
            .AddTo(gameObject);
    }

    protected void Return()
    {
        if (!gameObject.activeSelf)
            return;

        gameObject.SetActive(false);
        if (IsTokenCancellable)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
        ObjectPoolManager.Instance.Return(this);
    }
}