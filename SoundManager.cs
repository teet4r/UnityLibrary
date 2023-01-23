using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
    

    abstract class BaseAudio : MonoBehaviour
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

        protected AudioClip _clip = null;
        protected Dictionary<string, AudioClip> _clipDictionary = new Dictionary<string, AudioClip>();

        void Awake()
        {
            if (_audioSource == null)
            {
                if (TryGetComponent(out AudioSource audioSource))
                    _audioSource = audioSource;
                else
                    _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        public abstract void AddClip();
        public abstract void Play(string name);
    }

    class BgmAudio : BaseAudio
    {
        public override void AddClip()
        {
        }

        public override void Play(string name)
        {
        }
    }

    class SfxAudio : BaseAudio
    {
        public override void AddClip()
        {
            
        }

        public override void Play(string name)
        {
            if (_clipDictionary.TryGetValue(name, out _clip))
            {
                _audioSource.PlayOneShot(_clip);
                return;
            }
            Debug.LogError($"{name} sfx is not found.");
        }
    }
}
