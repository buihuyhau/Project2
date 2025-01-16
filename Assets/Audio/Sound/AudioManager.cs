using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public Sound[] musicSound, sfxSound;
    public AudioSource musicSource, sfxSource;

    private void Awake()
    {
        Instance = this;
        
    }


    private void Start()
    {
        PlayMusic("music0");

    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSound, x => x.name == name);
        if (s == null) { Debug.Log("sound not found"); }
        else
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
    }
    
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSound, x => x.name == name);
        if (s == null) { Debug.Log("sound not found"); }
        else
        {
            sfxSource.clip = s.clip;
            sfxSource.Play();
        }
    }
    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    public void SFXVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}
