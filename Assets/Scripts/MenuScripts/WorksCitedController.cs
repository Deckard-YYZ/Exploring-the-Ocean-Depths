using UnityEngine;
using UnityEngine.SceneManagement;

public class WorksCitedController : MonoBehaviour {
    public GameObject biblioCanvas;


    void Start() {
        //Hide the UI since the works cited is not open by default
        CloseBiblio();
    }

    public void OpenBiblio() {
        //Show the pause menu
        biblioCanvas.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseBiblio() {
        biblioCanvas.SetActive(false);
    }

}
