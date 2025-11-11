// AudioManager.cs (Corrected to load saved volumes on Start)
using System;
using UnityEngine;

// You still need your Sound.cs file for this to work
// [System.Serializable] public class Sound { ... }

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    // It's good practice to keep the keys consistent by defining them here
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

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

    private void Start()
    {
        // --- THIS IS THE CORRECTED/ADDED LOGIC ---
        // Load the saved volume settings from the last game session.
        // Use a default value (e.g., 0.75f) if no setting has ever been saved.
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, 0.75f);
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, 0.75f);

        // This log helps you confirm the values are being loaded correctly
        Debug.Log($"[AudioManager] Loading saved volumes. Music: {musicVolume}, SFX: {sfxVolume}");

        // Apply these loaded volumes directly to the AudioSources
        MusicVolume(musicVolume);
        SFXVolume(sfxVolume);
        // --- END OF CORRECTED LOGIC ---

        // Now, play the initial music track *after* the correct volume has been set.
        instance.PlayMusic("Theme");
        instance.MusicVolume(1.0f);  // 50% volume
    }

    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning($"Music '{name}' not found!");
            return;
        }
        if (musicSource == null)
        {
            Debug.LogError("Music source is not assigned!");
            return;
        }

        musicSource.clip = s.clip;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s == null)
        {
            Debug.LogWarning($"SFX '{name}' not found!");
            return;
        }
        if (sfxSource == null)
        {
            Debug.LogError("SFX source is not assigned!");
            return;
        }

        sfxSource.PlayOneShot(s.clip);
    }

    // --- Your control methods remain the same ---

    public void ToggleMusic()
    {
        if (musicSource != null)
            musicSource.mute = !musicSource.mute;
    }

    public void ToggleSFX()
    {
        if (sfxSource != null)
            sfxSource.mute = !sfxSource.mute;
    }

    public void MusicVolume(float volume)
    {
        if (musicSource != null)
            musicSource.volume = volume;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
    }

    public void SFXVolume(float volume)
    {
        if (sfxSource != null)
            sfxSource.volume = volume;
        PlayerPrefs.SetFloat(SFX_VOLUME_KEY, volume);
    }
}