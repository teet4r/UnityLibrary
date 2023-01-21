using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReturnToPool : MonoBehaviour
{
    [Min(0f)][SerializeField] float _returnTime;

    WaitForSecondsRealtime _wfsrt = null;

    void Awake()
    {
        _wfsrt = new WaitForSecondsRealtime(_returnTime);
    }

    void OnEnable()
    {
        StartCoroutine(_ReturnToPool());
    }

    void OnDisable()
    {
        _InstantReturnToPool();
    }

    IEnumerator _ReturnToPool()
    {
        yield return _wfsrt;

        _InstantReturnToPool();
    }

    void _InstantReturnToPool()
    {
        if (gameObject.activeSelf)
            PoolManager.instance.Put(gameObject);
    }
}
