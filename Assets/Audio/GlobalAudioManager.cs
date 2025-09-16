using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    public class GlobalAudioManager : MonoBehaviour
    {
        public List<Sound> sounds = new();
        [Range(0f, 1f)] public float masterVolume = 1f;
        
        public static GlobalAudioManager Instance { get; private set; }

        private AudioSource _audioSource;
        private Dictionary<string, Sound> _soundMap;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            _audioSource = GetComponent<AudioSource>();
            _soundMap = new Dictionary<string, Sound>();

            foreach (var sound in sounds)
            {
                if (sound.clip == null) continue;

                var key = sound.name ?? sound.clip.name;
                _soundMap[key] = sound;
            }
        }

        public void Play(string soundName)
        {
            if (_soundMap.TryGetValue(soundName, out var sound))
            {
                _audioSource.PlayOneShot(sound.clip, sound.volume * masterVolume);
            }
            else
            {
                Debug.LogWarning($"AudioManager: No sound named '{soundName}' found!");
            }
        }
    }
}