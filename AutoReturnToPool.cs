using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AutoReturnToPool : MonoBehaviour
{
    [Min(0f)][SerializeField] float _returnTime = Mathf.Infinity;

    WaitForSeconds _wfsrt = null;
    Coroutine _returnCor = null;

    void Awake()
    {
        SceneManager.sceneLoaded += OnChangeScene;

        _wfsrt = new WaitForSeconds(_returnTime);
    }

    void OnEnable()
    {
        StartTimer();
    }

    void OnDisable()
    {
        StopTimer();
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnChangeScene;
    }

    void StartTimer()
    {
        if (_returnCor != null) return;

        _returnCor = StartCoroutine(_ReturnToPool());
    }

    void StopTimer()
    {
        if (_returnCor == null) return;

        StopCoroutine(_returnCor);
        _returnCor = null;
    }

    void OnChangeScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        StopTimer();

        PoolManager.Instance.Put(gameObject);
    }

    IEnumerator _ReturnToPool()
    {
        yield return _wfsrt;

        PoolManager.Instance.Put(gameObject);
    }
}
