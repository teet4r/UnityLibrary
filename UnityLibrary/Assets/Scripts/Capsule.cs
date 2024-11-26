using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class Capsule : PoolObject
{
    public async UniTask ReturnAsync()
    {
        await UniTask.Delay(3000, cancellationToken: CancellationTokenSource.Token);
        Return();
    }
}
