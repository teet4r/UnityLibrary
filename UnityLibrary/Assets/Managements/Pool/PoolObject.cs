using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObject : MonoBehaviour
{
    public Transform Tr => tr;
    protected Transform tr;

    public Action OnReturn;

    protected virtual void Awake()
    {
        tr = GetComponent<Transform>();
    }
}