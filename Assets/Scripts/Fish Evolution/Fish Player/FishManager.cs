using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FishManager : MonoBehaviour
{
    public static int fishIndex;
    InputManager inputManager;
    CameraManager cameraManager;
    FishMovement fishMovement;
    public GameObject fishUI;
    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        fishMovement = GetComponent<FishMovement>();
    }

    private void Update()
    {
        inputManager.HandleAllInputs();
        HandleFishSwitching();
    }

    private void FixedUpdate()
    {
        fishMovement.HandleAllMovement();
    }
    private void LateUpdate()
    {
        cameraManager.HandleAllCameraMovement();
    }

    private void HandleFishSwitching()
    {
        bool[] fishFlags = inputManager.fishFlags;
        for (int i =0; i < fishFlags.Length; i++)
        {
            if (fishFlags[i])
            {
                fishIndex = i;
                transform.GetChild(i).gameObject.SetActive(true);
                var fishUIImage = fishUI.transform.GetChild(i).GetComponent<Image>().color;
                fishUIImage.a = 1f;
                fishUI.transform.GetChild(i).GetComponent<Image>().color = fishUIImage;
                fishMovement.setSpeed(transform.GetChild(i).GetComponent<FishStats>().speed);
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(false);
                var fishUIImage = fishUI.transform.GetChild(i).GetComponent<Image>().color;
                fishUIImage.a = 0.4f;
                fishUI.transform.GetChild(i).GetComponent<Image>().color = fishUIImage;
            }
        }
    }
}
