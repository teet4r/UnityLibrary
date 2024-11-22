using UnityEngine;

public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance => _instance;
    private static T _instance;
    public virtual bool IsLoaded => !_instance.IsNull();

    protected virtual void Awake()
    {
        _instance = this as T;

        DontDestroyOnLoad(gameObject);
    }
}