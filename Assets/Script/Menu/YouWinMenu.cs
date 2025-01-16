//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class YouWinMenu : MonoBehaviour
//{
//    protected bool temp = true;
//    public GameObject winMenuUI;
//    private void Update()
//    {
//        if (ShipCtrl.Instance.Inventory.ore >= 10)
//        {
//            if (temp) 
//            {
//                temp = false;
//                AudioManager.Instance.PlayMusic("win");

//            }
//            Time.timeScale = 0f;
//            winMenuUI.SetActive(true);
//            PlayerPrefs.SetFloat("SavedTime", 0);
//            PlayerPrefs.SetInt("SavedLevel", 1);
//        }
//    }

//    public void LoadMenu()
//    {
//        Time.timeScale = 1f;
//        SceneManager.LoadScene("MainMenu");
//    }
//    public void QuitGame()
//    {
//        Application.Quit();
//    }
//}
