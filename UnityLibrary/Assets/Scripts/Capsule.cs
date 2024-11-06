using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Capsule : PoolObject
{
    public async UniTask Return()
    {
        await UniTask.Delay(3000);
        ObjectPoolManager.Instance.Return(this);
    }
}
