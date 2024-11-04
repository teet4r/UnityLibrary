using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public abstract class PoolObject : MonoBehaviour
{
    public Transform Tr => tr;
    protected Transform tr;

    protected CancellationToken disableCancellationToken => _cancellation.Token;
    private CancellationTokenSource _cancellation;

    protected virtual void Awake()
    {
        tr = GetComponent<Transform>();
    }

    protected virtual void OnEnable()
    {
        _cancellation = new CancellationTokenSource();
    }

    protected virtual void OnDisable()
    {
        _cancellation.Cancel();
        _cancellation.Dispose();
    }
}