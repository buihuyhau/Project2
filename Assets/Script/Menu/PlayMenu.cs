using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenu : MonoBehaviour
{
    public void PlayNewGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadSceneAsync(1);
        PlayerPrefs.SetInt("SavedLevel", 1);
        PlayerPrefs.SetFloat("SavedTime", 0);
    }
    public void ContinuePlayGame()
    {
         Time.timeScale = 1f;

        if (PlayerPrefs.GetInt("SavedLevel") == 0)
        {
            SceneManager.LoadSceneAsync(1);

        }
        else
        {

            SceneManager.LoadSceneAsync(PlayerPrefs.GetInt("SavedLevel"));
        }
    }
}
