using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance
    {
        get
        {
            if (_instance == null && Time.timeScale != 0f)
            {
                _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    var newObj = new GameObject();
                    _instance = newObj.AddComponent<T>();
                    newObj.name = typeof(T).ToString();
                    DontDestroyOnLoad(newObj);
                }
                else
                    DontDestroyOnLoad(_instance.gameObject);
            }
            return _instance;
        }
    }
    static T _instance = null;

    void OnApplicationQuit()
    {
        Time.timeScale = 0f;
    }
}