using System;
using System.Collections;
using UnityEngine;

public enum SoundType
{
    Music,       // Soundtracks (only one plays at a time, with crossfade)
    Background,  // Ambience: wind, rain, crowd... (can run alongside music)
    SFX          // Short effects: jump, shot, explosion... (can overlap)
}

[Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    public SoundType type = SoundType.SFX;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;
    public bool loop;

    [HideInInspector] public AudioSource source; // created at runtime in Awake
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Sounds (fill these in the Inspector)")]
    public Sound[] sounds;

    [Header("Volumes (0 - 1)")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float backgroundVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    [Header("Music")]
    public float musicFadeTime = 1.5f;
    

    private Sound currentMusic;
    private Coroutine musicRoutine;

    // ---------------------------------------------------------------
    // SETUP
    // ---------------------------------------------------------------
    private void Awake()
    {
        // Singleton: only one AudioManager can exist
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // survives scene changes (music doesn't cut)

        // "Instantiate" each sound: one AudioSource per Sound, added to this object
        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = false;
            s.source.volume = s.type == SoundType.SFX ? 1f : GetFinalVolume(s);
        }
    }

    private void Start()
    {
        Play("playOnStart");
    }

    // ---------------------------------------------------------------
    // PUBLIC API
    // ---------------------------------------------------------------
    public void Play(string soundName)
    {
        Sound s = Find(soundName);
        if (s == null) return;

        switch (s.type)
        {
            case SoundType.SFX:
                if (s.loop)
                {
                    // SFX en loop (pasos, motor...): PlayOneShot ignora el loop, así que usamos Play()
                    if (!s.source.isPlaying)
                    {
                        s.source.volume = GetFinalVolume(s);
                        s.source.pitch = s.pitch;
                        s.source.Play();
                    }
                }
                else
                {
                    // SFX normales: pueden solaparse consigo mismos
                    s.source.pitch = s.pitch;
                    s.source.PlayOneShot(s.clip, GetFinalVolume(s));
                }
                break;

            case SoundType.Music:
                PlayMusic(s);
                break;

            case SoundType.Background:
                if (!s.source.isPlaying)
                {
                    s.source.volume = GetFinalVolume(s);
                    s.source.Play();
                }
                break;
        }
    }

    public void Stop(string soundName)
    {
        Sound s = Find(soundName);
        if (s == null) return;

        if (s == currentMusic)
        {
            if (musicRoutine != null) StopCoroutine(musicRoutine);
            currentMusic = null;
        }
        s.source.Stop();
    }

    public void StopAll()
    {
        if (musicRoutine != null) StopCoroutine(musicRoutine);
        currentMusic = null;
        foreach (Sound s in sounds) s.source.Stop();
    }

    public bool IsPlaying(string soundName)
    {
        Sound s = Find(soundName);
        return s != null && s.source.isPlaying;
    }

    // Volume setters (hook these to UI sliders)
    public void SetMasterVolume(float v) { masterVolume = Mathf.Clamp01(v); RefreshVolumes(); }
    public void SetMusicVolume(float v) { musicVolume = Mathf.Clamp01(v); RefreshVolumes(); }
    public void SetBackgroundVolume(float v) { backgroundVolume = Mathf.Clamp01(v); RefreshVolumes(); }
    public void SetSFXVolume(float v) { sfxVolume = Mathf.Clamp01(v); } // SFX read the volume each time they play

    // ---------------------------------------------------------------
    // INTERNALS
    // ---------------------------------------------------------------
    private Sound Find(string soundName)
    {
        Sound s = Array.Find(sounds, x => x.name == soundName);
        if (s == null)
            Debug.LogWarning($"AudioManager: sound '{soundName}' not found.");
        return s;
    }

    private float GetFinalVolume(Sound s)
    {
        float category = s.type == SoundType.Music ? musicVolume
                       : s.type == SoundType.Background ? backgroundVolume
                       : sfxVolume;
        return s.volume * category * masterVolume;
    }

    private void RefreshVolumes()
    {
        foreach (Sound s in sounds)
        {
            if (s.type == SoundType.SFX) continue;
            // Don't fight the crossfade while it's running
            if (musicRoutine != null && s.type == SoundType.Music) continue;
            if (s.source.isPlaying) s.source.volume = GetFinalVolume(s);
        }
    }

    private void PlayMusic(Sound next)
    {
        if (currentMusic == next && next.source.isPlaying) return;

        if (musicRoutine != null) StopCoroutine(musicRoutine);

        // Cut any music that is not the one we're about to fade out
        foreach (Sound s in sounds)
            if (s.type == SoundType.Music && s != currentMusic && s != next)
                s.source.Stop();

        musicRoutine = StartCoroutine(CrossfadeMusic(next));
    }

    private IEnumerator CrossfadeMusic(Sound next)
    {
        Sound previous = currentMusic;
        currentMusic = next;

        float targetVolume = GetFinalVolume(next);
        float previousStart = previous != null ? previous.source.volume : 0f;

        next.source.volume = 0f;
        next.source.Play();

        float t = 0f;
        while (t < musicFadeTime)
        {
            t += Time.unscaledDeltaTime; // keeps working if Time.timeScale = 0 (pause)
            float k = Mathf.Clamp01(t / musicFadeTime);
            next.source.volume = Mathf.Lerp(0f, targetVolume, k);
            if (previous != null) previous.source.volume = Mathf.Lerp(previousStart, 0f, k);
            yield return null;
        }

        next.source.volume = targetVolume;
        if (previous != null) previous.source.Stop();
        musicRoutine = null;
    }

    public void playGameplayMusic ()
    {
        Stop("playOnStart");
        Play("gameplayMusic");
    }

    public void playNavigating()
    {
        Play("navigating");
    }

    public void playBeep()
    {
        Play("downSound");
    }

    public void playDiving()
    {
        Play("diving");
    }

    public void StopNavigating()
    {
        Stop("navigating");
    }

    public void StopBeep()
    {
        Stop("downSound");
    }

    public void StopDiving()
    {
        Stop("diving");
    }

    public void playPick()
    {
        Play("pick");
    }
    public void StopPick()
    {
        Stop("pick");
    }

    public void playDrop()
    {
        Play("dropWaste");
    }

    public void StopDrop()
    {
        Stop("dropWaste");
    }
}