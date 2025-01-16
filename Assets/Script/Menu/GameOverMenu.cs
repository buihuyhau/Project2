using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenu : MonoBehaviour
{
    public void ReTry()
    {
        Time.timeScale = 1f;
        ResetPlayerState();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
        ResetPlayerState();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void ResetPlayerState()
    {
        var player = FindObjectOfType<MovementController>();
        if (player != null)
        {
            player.gameObject.SetActive(true);
            player.enabled = true;
            player.GetComponent<BombController>().enabled = true;
            player.spriteRendererDeath.enabled = false;
            player.spriteRendererDown.enabled = true;
            player.transform.position = Vector2.zero; // Reset player position or set to a specific spawn point
            //player.direction = Vector2.down; // Reset direction
        }
    }

}
