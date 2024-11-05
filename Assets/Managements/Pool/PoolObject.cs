using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolObject : MonoBehaviour
{
    public Transform Tr => tr;
    protected Transform tr;

    protected virtual void Awake()
    {
        tr = GetComponent<Transform>();
    }
}