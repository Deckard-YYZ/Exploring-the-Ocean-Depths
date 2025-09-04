using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public GameObject mainMenuCanavas;
    public GameObject loadingCanvas;

    public void Start()
    {
        mainMenuCanavas.SetActive(true); 
        loadingCanvas.SetActive(false);
    }
    public void LoadScene(string sceneName)
    {
        loadingCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMainMenu()
    {
        mainMenuCanavas.SetActive(true);
    }

    public void PrintMessage(string msg)
    {
        Debug.Log(msg);
    }

    public void QuitGame()
    {
        Debug.Log("Player Quit Game");
        Application.Quit();
    }
}
