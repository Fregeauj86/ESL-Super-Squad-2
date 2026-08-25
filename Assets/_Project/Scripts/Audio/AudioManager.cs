using UnityEngine;

namespace FromCell.Audio
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField] AudioSource musicSource;
        [SerializeField] AudioSource sfxSource;
        [Range(0f, 1f)] [SerializeField] float musicVolume = 0.6f;
        [Range(0f, 1f)] [SerializeField] float sfxVolume = 0.85f;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
            }

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
            }

            ApplyVolumes();
        }

        public void PlayJump() => PlaySfx(null);
        public void PlayEvolution() => PlaySfx(null);
        public void PlayDeath() => PlaySfx(null);

        public void PlaySfx(AudioClip clip)
        {
            if (clip == null || sfxSource == null) return;
            sfxSource.PlayOneShot(clip, sfxVolume);
        }

        public void PlayMusic(AudioClip clip)
        {
            if (musicSource == null) return;

            musicSource.clip = clip;
            musicSource.volume = musicVolume;
            if (clip != null)
                musicSource.Play();
            else
                musicSource.Stop();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            if (musicSource != null)
                musicSource.volume = musicVolume;
        }

        public void SetSfxVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }

        void ApplyVolumes()
        {
            if (musicSource != null)
                musicSource.volume = musicVolume;
        }
    }
}
