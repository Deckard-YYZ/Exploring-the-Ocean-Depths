using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EnterFishMinigame : MonoBehaviour
{
    public GameObject enterMinigamePanel;
    //public TMP_Text gameTitleText;
    public TMP_Text gameDescriptionText;

    private void Start()
    {
        enterMinigamePanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player hits fish minigame cube");
            enterMinigamePanel.SetActive(true);
            //gameTitleText.text = "Fish Discovery";
            gameDescriptionText.text = "You warp into a fish! Try to find other fish and evolve into the Shark to survive!";

            //Allow the player to move the cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
    }

    public void ClosePanel()
    {
        enterMinigamePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void StartFishGame()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        SceneManager.LoadScene("Fish Evolution");
    }
}
