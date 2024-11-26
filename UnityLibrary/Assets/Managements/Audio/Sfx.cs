using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Sfx : MonoBehaviour
{
    public bool IsLoaded => !_audioSource.IsNull();

    private AudioSource _audioSource;
    private Dictionary<SfxName, AudioClip> _sfxs = new();

    public float Volume
    {
        get => _audioSource.volume;
        set
        {
            _audioSource.volume = value;
            PlayerPrefs.SetFloat("SfxVolume", value);
            PlayerPrefs.Save();
        }
    }

    private void Awake()
    {
        if (TryGetComponent(out _audioSource))
            _audioSource.volume = PlayerPrefs.GetFloat("SfxVolume", 1f);

    }

    public void Play(SfxName sfx)
    {
        if (!_sfxs.TryGetValue(sfx, out AudioClip clip))
        {
            var obj = Addressables.LoadAssetAsync<AudioClip>(sfx.ToString()).WaitForCompletion();
            _sfxs.Add(sfx, clip = obj);
        }
        _audioSource.PlayOneShot(clip);
    }
}
