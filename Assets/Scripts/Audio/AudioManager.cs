using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public enum Category
    {
        Master,
        Music,
        Ambience,
        SFX
    }

    [Header("Volume Props")]
    [SerializeField, Range(0f, 1f)] private float masterVolume = 1f;
    [SerializeField, Range(0f, 1f)] private float musicVolume = .8f;
    [SerializeField, Range(0f, 1f)] private float ambienceVolume = .8f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    [Header("SFX Pool")]
    [SerializeField] private int sfxPoolSize = 10;

    private AudioSource musicSource;
    private AudioSource ambienceSource;
    private readonly List<AudioSource> sfxPool = new List<AudioSource>();
    private int sfxIndex;
    private Coroutine musicFade;

    private float MusicLevel => musicVolume * masterVolume;
    private float AmbienceLevel => ambienceVolume * masterVolume;
    private float SfxLevel => sfxVolume * masterVolume;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        // Creating GameObjects with Audio Source Attatched
        musicSource = CreateAudioSourceObject("Music", true);
        ambienceSource = CreateAudioSourceObject("Ambience", true);

        for (int i = 0; i < sfxPoolSize; i++)
            sfxPool.Add(CreateAudioSourceObject($"SFX {i}", false));
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
            return;

        var src = sfxPool[sfxIndex];
        sfxIndex = (sfxIndex + 1) % sfxPool.Count;

        src.clip = clip;
        src.volume = SfxLevel;
        src.Play();
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1f)
    {
        if (clip == null || musicSource.clip == clip)
            return;

        if (musicFade != null)
            StopCoroutine(musicFade);

        musicFade = StartCoroutine(CrossfadeMusic(clip, fadeDuration));
    }

    // Can be buggy, if needed refractor this.
    private IEnumerator CrossfadeMusic(AudioClip clip, float duration)
    {
        if (musicSource.isPlaying)
        {
            float start = musicSource.volume;
            for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(start, 0f, t / duration);
                yield return null;
            }
        }

        musicSource.clip = clip;
        musicSource.Play();

        for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
        {
            musicSource.volume = Mathf.Lerp(0f, MusicLevel, t / duration);
            yield return null;
        }

        musicSource.volume = MusicLevel;
        musicFade = null;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PlayAmbience(AudioClip clip)
    {
        if (clip == null)
            return;

        ambienceSource.clip = clip;
        ambienceSource.volume = AmbienceLevel;
        ambienceSource.Play();
    }

    public void StopAmbience()
    {
        ambienceSource.Stop();
    }


    private AudioSource CreateAudioSourceObject(string name, bool loop)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform);

        var src = go.AddComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = loop;
        return src;
    }
}