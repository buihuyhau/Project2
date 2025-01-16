
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingMenu : MonoBehaviour
{
    public Slider _musicSlider, _sfxSlider;
    private void Start()
    {
        _musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        _sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
    }
    public void MusicVolum()
    {
        PlayerPrefs.SetFloat("musicVolume", _musicSlider.value);
        AudioManager.Instance.MusicVolume(_musicSlider.value);
    }
    public void SFXVolum()
    {
        PlayerPrefs.SetFloat("sfxVolume", _sfxSlider.value);

        AudioManager.Instance.SFXVolume(_sfxSlider.value);
    }
}