using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    public BgmAudio BgmAudio { get; private set; } = null;
    public SfxAudio SfxAudio { get; private set; } = null;

    bool _isCreated = false;

    protected override void Awake()
    {
        base.Awake();

        Create();
    }

    public void Create()
    {
        if (_isCreated) return;
        _isCreated = true;

        // BgmAudio 생성
        var newObj0 = new GameObject("BgmAudio");
        newObj0.transform.parent = transform;
        BgmAudio = newObj0.AddComponent<BgmAudio>();
        
        // SfxAudio 생성
        var newObj1 = new GameObject("SfxAudio");
        newObj1.transform.parent = transform;
        SfxAudio = newObj1.AddComponent<SfxAudio>();
    }
}

public abstract class BaseAudio : MonoBehaviour
{
    public bool IsMute
    {
        get { return _audioSource.mute; }
        set { _audioSource.mute = value; }
    }
    public float Volume
    {
        get { return _audioSource.volume; }
        set { _audioSource.volume = value; }
    }

    [SerializeField] protected AudioSource _audioSource = null;

    protected virtual void Awake()
    {
        if (_audioSource == null)
        {
            if (TryGetComponent(out AudioSource audioSource))
                _audioSource = audioSource;
            else
                _audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public abstract void Play(string name);
}

public class BgmAudio : BaseAudio
{
    protected override void Awake()
    {
        base.Awake();

        _audioSource.playOnAwake = false;
        _audioSource.loop = true;
    }

    public override void Play(string name)
    {
        var clip = ResourceManager.Instance.Get<AudioClip>(name);
        if (clip == null)
        {
            Debug.LogError($"{name} bgm is not found.");
            return;
        }
        _audioSource.clip = clip;
        _audioSource.Play();
    }
}

public class SfxAudio : BaseAudio
{
    protected override void Awake()
    {
        base.Awake();

        _audioSource.playOnAwake = false;
    }

    public override void Play(string name)
    {
        var clip = ResourceManager.Instance.Get<AudioClip>(name);
        if (clip == null)
        {
            Debug.LogError($"{name} sfx is not found.");
            return;
        }
        _audioSource.PlayOneShot(clip);
    }
}