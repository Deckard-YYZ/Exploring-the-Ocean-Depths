using System.Collections;
using System.Collections.Generic;
// using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class FirstPersonCamera : MonoBehaviour {
    public Transform player;
    public float mouseSensitivity = 2f;
    public Slider sensSlider;

    public Transform orientation;

    float xRotation = 0f;
    float yRotation = 0f;

    private void Start() {
        // hide the cursor and lock it in the middle of the screen
        //NOTE: When these are active, menus are not navigable. Toggle these through the code that opens/closes menus instead
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update() {
        float inputX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float inputY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yRotation += inputX;

        xRotation -= inputY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); //clamp the rotation so the user can't look too far up or down

        // rotate the camera
        transform.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        // rotate the object that stores what direction we're facing
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
        //rotate the player so that they can't turn around and see themself
        player.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void UpdateSensitivityNew(System.Single sens)
    {
        mouseSensitivity = sens;
    }

}
