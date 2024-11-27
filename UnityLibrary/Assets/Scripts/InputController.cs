using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    [SerializeField] private PoolObject _tri;
    [SerializeField] private PoolObject _cap;
    [Min(0f)][SerializeField] private int _delay;

    [SerializeField] private Button _start;
    [SerializeField] private Button _stop;
    [SerializeField] private Button _init;
    [SerializeField] private Button _return;
    [SerializeField] private Button _pause;
    [SerializeField] private Button _goNextScene;

    private CancellationTokenSource _cancellationTokenSource;

    private void Awake()
    {
        _start.onClick.AddListener(() => _Run1().Forget());
        _stop.onClick.AddListener(() =>
        {
            if (_cancellationTokenSource == null)
                return;

            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        });
        _init.onClick.AddListener(() => ObjectPoolManager.Instance.ClearAll());
        _return.onClick.AddListener(() => ObjectPoolManager.Instance.HideAll());
        _pause.onClick.AddListener(() => Time.timeScale = Time.timeScale == 0f ? 1f : 0f);
        _goNextScene.onClick.AddListener(() =>
        {
            SceneManager.Instance.LoadSceneAsync(
                SceneName.NextScene,
                async () =>
                {
                    await UniTask.Delay(3000);
                    ObjectPoolManager.Instance.ClearAll();
                }).Forget();
        });
    }

    private void Start()
    {
        AudioManager.Instance.Bgm.Play(BgmName.Track5);
    }

    private async UniTask _Run1()
    {
        if (_cancellationTokenSource == null)
            _cancellationTokenSource = new();

        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var capsule = ObjectPoolManager.Instance.Get<Capsule>();
            var triangle = ObjectPoolManager.Instance.Get<Triangle>();

            Vector2 pos = new Vector2(Random.Range(-1f, 1f), 1f);

            capsule.Tr.position = pos;
            triangle.Tr.position = pos;

            capsule.ReturnAsync().Forget();
            triangle.ReturnAsync().Forget();

            await UniTask.Delay(_delay, cancellationToken: _cancellationTokenSource.Token);
        }
    }

    private async UniTask _Run2()
    {
        Vector2 pos = Vector2.zero;
        while (true)
        {
            Instantiate(_tri, Vector2.zero, Quaternion.identity);
            Instantiate(_cap, Vector2.zero, Quaternion.identity);

            await UniTask.Delay(_delay);
        }
    }

    private async UniTask _Run3()
    {
        while (true)
        {
            var handle = Addressables.InstantiateAsync("Capsule", Vector2.zero, Quaternion.identity).WaitForCompletion().GetComponent<PoolObject>();
            var handle2 = Addressables.InstantiateAsync("Triangle", Vector2.zero, Quaternion.identity).WaitForCompletion().GetComponent<PoolObject>();

            handle.Tr.position = Vector2.zero;
            handle2.Tr.position = Vector2.zero;

            await UniTask.Delay(_delay);
        }
    }
}
