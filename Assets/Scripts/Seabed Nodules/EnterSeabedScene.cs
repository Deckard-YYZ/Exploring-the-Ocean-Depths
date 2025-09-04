using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterSeabedScene : MonoBehaviour {
    public GameObject enterMinigamePanel;
   // public TMP_Text gameTitleText;
    public TMP_Text gameDescriptionText;

    private void Start() {
        enterMinigamePanel.SetActive(false);
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            Debug.Log("Player hits seabed mini game");
            enterMinigamePanel.SetActive(true);
           // gameTitleText.text = "Manganese nodules";
            gameDescriptionText.text = "This part of the seabed is full of manganese nodules. They're full of useful metals, so try to collect as many as possible!";

            //Allow the player to move the cursor
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0;
        }
    }

    public void ClosePanel() {
        enterMinigamePanel.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
    }

    public void StartNoduleGame() {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        SceneManager.LoadScene("Seabed Nodules");
    }
}
