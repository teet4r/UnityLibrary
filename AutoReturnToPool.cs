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

    IEnumerator _ReturnToPool()
    {
        yield return _wfsrt;

        PoolManager.instance.Put(gameObject);
    }
}
