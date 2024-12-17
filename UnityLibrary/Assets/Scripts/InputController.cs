using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    public static InputController Instance => _instance;
    private static InputController _instance;

    [SerializeField] private PoolObject _tri;
    [SerializeField] private PoolObject _cap;
    [Min(0f)][SerializeField] private int _delay;

    [SerializeField] private Button _start;
    [SerializeField] private Button _stop;
    [SerializeField] private Button _init;
    [SerializeField] private Button _return;
    [SerializeField] private Button _pause;
    [SerializeField] private Button _goNextScene;
    [SerializeField] private Text _time;

    private CancellationTokenSource _cancellationTokenSource;
    private ReactiveProperty<bool> _onNextScene = new(false);

    private void Awake()
    {
        _instance = this;

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
            AddSubcriptionOnNextScene(invoke =>
            {
                if (invoke)
                {
                    _stop.onClick.Invoke();
                    ObjectPoolManager.Instance.ClearAll();
                }
            });

            SceneManager.Instance.LoadSceneAsync(
                SceneName.NextScene,
                async () =>
                {
                    _onNextScene.Value = true;
                    await UniTask.Delay(3000);
                }
            ).Forget();
        });
    }

    private void Start()
    {
        AudioManager.Instance.Bgm.Play(BgmName.Track5);

        float t = 0;
        Observable.EveryUpdate()
            .Subscribe(_ =>
            {
                t += Time.deltaTime;
                _time.text = $"Time: {t:N2}";
            })
            .AddTo(gameObject);
    }

    public void AddSubcriptionOnNextScene(Action<bool> onNextScene)
    {
        _onNextScene.Subscribe(onNextScene)
            .AddTo(gameObject);
    }

    private async UniTask _Run1()
    {
        if (_cancellationTokenSource == null)
            _cancellationTokenSource = new();

        while (!_cancellationTokenSource.IsCancellationRequested)
        {
            var capsule = ObjectPoolManager.Instance.Get("Capsule") as Capsule;
            var triangle = ObjectPoolManager.Instance.Get("Triangle") as Triangle;

            Vector2 pos = new Vector2(UnityEngine.Random.Range(-1f, 1f), 1f);

            capsule.transform.position = pos;
            triangle.transform.position = pos;

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

            handle.transform.position = Vector2.zero;
            handle2.transform.position = Vector2.zero;

            await UniTask.Delay(_delay);
        }
    }
}
