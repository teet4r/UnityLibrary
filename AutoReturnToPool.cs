using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoReturnToPool : MonoBehaviour
{
    [Min(0f)][SerializeField] float _returnTime;

    WaitForSeconds _wfsrt = null;
    Coroutine _returnCor = null;

    void Awake()
    {
        _wfsrt = new WaitForSeconds(_returnTime);
    }

    void OnEnable()
    {
        _returnCor = StartCoroutine(_ReturnToPool());
    }

    void OnDisable()
    {
        if (_returnCor == null) return;

        StopCoroutine(_returnCor);
        _returnCor = null;
    }

    IEnumerator _ReturnToPool()
    {
        yield return _wfsrt;

        PoolManager.instance.Put(gameObject);
    }
}
