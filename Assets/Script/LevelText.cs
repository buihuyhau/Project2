using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelText : MonoBehaviour
{
    public GameObject levelText;
    protected  void Start()
    {
        Invoke("SetActText", 1f);
        AudioManager.Instance.MusicVolume(PlayerPrefs.GetFloat("musicVolume"));
        AudioManager.Instance.SFXVolume(PlayerPrefs.GetFloat("sfxVolume"));
        AudioManager.Instance.PlayMusic("music" + SceneManager.GetActiveScene().buildIndex.ToString());

    }
    protected virtual void SetActText()
    {
        levelText.SetActive(false);
    }
}
