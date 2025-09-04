using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartUI : MonoBehaviour
{
    public static bool gameStarted = false;
    public GameObject inGamePanel;

    private void Update()
    {
        if (!gameStarted && !Cursor.visible)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
    public void StartMinigame()
    {
        gameStarted = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        gameObject.SetActive(false);
        inGamePanel.SetActive(true);
    }
}
