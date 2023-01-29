using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 풀 매니저에서 관리하기 용이하도록
/// 모든 GameObject prefab은 해당 스크립트를 가져야 함.
/// </summary>
public class AutoReturnToPool : MonoBehaviour
{
    public float ReturnTime
    {
        get { return _returnTime; }
    }

    [Min(0f)][SerializeField] float _returnTime = Mathf.Infinity;

    WaitForSeconds _wfsReturnTime = null;
    Coroutine _returnCor = null;

    void Awake()
    {
        SceneManager.sceneLoaded += OnChangeScene;

        _wfsReturnTime = new WaitForSeconds(_returnTime);
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
        PoolManager.Instance.Put(gameObject);
    }
    
    /// <summary>
    /// 씬이 변경되면 모든 활성화된 프리팹을 풀에 반환
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="loadSceneMode"></param>
    void OnChangeScene(Scene scene, LoadSceneMode loadSceneMode)
    {
        StopTimer();
    }

    IEnumerator _ReturnToPool()
    {
        yield return _wfsReturnTime;

        PoolManager.Instance.Put(gameObject);
    }
}
