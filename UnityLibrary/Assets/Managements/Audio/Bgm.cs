using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Bgm : MonoBehaviour
{
    public bool IsLoaded => !_audioSource.IsNull();
    public bool IsPlaying => _audioSource.isPlaying;

    private AudioSource _audioSource;
    private Dictionary<BgmName, AudioClip> _bgms = new();

    public float Volume
    {
        get => _audioSource.volume;
        set
        {
            _audioSource.volume = value;
            PlayerPrefs.SetFloat("BgmVolume", value);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        if (TryGetComponent(out _audioSource))
            _audioSource.volume = PlayerPrefs.GetFloat("BgmVolume", 1f);
    }

    public void Play(BgmName bgm)
    {
        if (!_bgms.TryGetValue(bgm, out AudioClip clip))
        {
            var obj = Addressables.LoadAssetAsync<AudioClip>(bgm.ToString()).WaitForCompletion();
            _bgms.Add(bgm, clip = obj);
        }

        if (clip == _audioSource.clip)
            return;

        _audioSource.clip = clip;
        _audioSource.Play();
    }

    public void Stop()
    {
        if (!_audioSource.isPlaying)
            return;

        _audioSource.Stop();
    }
}
