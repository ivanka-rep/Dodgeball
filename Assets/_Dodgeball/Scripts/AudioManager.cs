using UnityEngine;
using System.Collections.Generic;

//Audio Manager will manipulate all sounds in the game

public class AudioManager : MonoBehaviour
{
    public static AudioManager s_instance;
    
    [SerializeField] private AudioSource m_musicAudioSource;
    [SerializeField] private AudioSource m_gameAudioeSource;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip m_clickSound;
    [SerializeField] private AudioClip m_menuMusic;

    public enum AudioType { MenuMusic, Click }

    public AudioSource MusicAudioSource 
    {
        get => m_musicAudioSource;
        set => m_musicAudioSource = value;
    }

    public AudioSource GameAudioSource
    {
        get => m_gameAudioeSource;
        set => m_gameAudioeSource = value;
    }

    void Awake()
    {
        if (s_instance == null) { DontDestroyOnLoad(gameObject); s_instance = this; }
        else if (s_instance != this) { Destroy(gameObject); }
    }

    public void Mute()
    {
        m_musicAudioSource.mute = true;
        m_gameAudioeSource.mute = true;
    }

    public void Unmute()
    {
        m_musicAudioSource.mute = false;
        m_gameAudioeSource.mute = false;
    }

    public void PlayMusic(AudioType source)
    {
        AudioClip clip = m_menuMusic;
        m_musicAudioSource.loop = true;
        m_musicAudioSource.volume = GameData.s_instance.MusicVolume;
        switch (source)
        {
            case AudioType.MenuMusic:
                clip = m_menuMusic;
                break;
        }

        m_musicAudioSource.Stop();
        m_musicAudioSource.clip = clip;
        m_musicAudioSource.Play();
    }

    public void PlaySoundEffect(AudioType effect)
    {
        AudioClip clip = null;
        m_gameAudioeSource.volume = GameData.s_instance.GameVolume;
        switch (effect)
        {
            case AudioType.Click:
                clip = m_clickSound;
                break;
        }

        m_gameAudioeSource.PlayOneShot(clip);
    }
}
