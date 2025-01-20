using System.Collections;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Tracks")]
    [SerializeField] private AudioClip menuMusic;
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private AudioClip bossMusic;
    [SerializeField] private AudioClip enemyDetectedMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicSource.isPlaying)
        {
            musicSource.Stop();
        }
        musicSource.clip = musicClip;
        musicSource.Play();
    }

    public void PlayMenuMusic()
    {
        PlayMusic(menuMusic);
    }

    public void PlayBackgroundMusic()
    {
        PlayMusic(backgroundMusic);
    }

    public void PlayBossMusic()
    {
        PlayMusic(bossMusic);
    }

    public void PlayEnemyDetectedMusic()
    {
        PlayMusic(enemyDetectedMusic);
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayMusicWithFade(AudioClip musicClip, float fadeDuration = 1f)
    {
        StartCoroutine(FadeMusic(musicClip, fadeDuration));
    }

    private IEnumerator FadeMusic(AudioClip newClip, float duration)
    {
        float startVolume = musicSource.volume;
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.Play();

        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            musicSource.volume = Mathf.Lerp(0, startVolume, t / duration);
            yield return null;
        }
    }
}
