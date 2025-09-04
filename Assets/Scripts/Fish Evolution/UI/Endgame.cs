using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Endgame : MonoBehaviour
{
    public GameObject ingamePanel;
    public GameObject endgamePanel;
    public GameObject timer;
    public Text scoreText;

    public void EndMinigame()
    {
        Time.timeScale = 0f;
        float timeScore = timer.GetComponent<Timer>().time;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        ingamePanel.SetActive(false);
        endgamePanel.SetActive(true);

        scoreText.text = $"Score: {timeScore}";
    }

    public void ContinueGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        ingamePanel.SetActive(true);
        endgamePanel.SetActive(false);
    }
    
    public void Restart()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void MainGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(0);
    }
}
