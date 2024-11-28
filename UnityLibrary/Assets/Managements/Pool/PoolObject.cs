using System.Threading;
using UniRx;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public Transform Tr => tr;
    protected Transform tr;

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
        tr = transform;

        ObjectPoolManager.Instance.OnHideOrClear.Subscribe(_ => Return())
            .AddTo(gameObject);
    }

    protected virtual void Return()
    {
        if (IsTokenCancellable)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }
        ObjectPoolManager.Instance.Return(this);
    }
}