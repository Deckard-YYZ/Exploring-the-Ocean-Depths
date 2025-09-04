using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettinsController : MonoBehaviour
{
    public GameObject settingsCanvas;
    void Start()
    {
        //Hide the UI since the works cited is not open by default
        CloseSettings();
    }

    public void OpenSettings()
    {
        //Show the pause menu
        settingsCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseSettings()
    {
        settingsCanvas.SetActive(false);
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }
}
