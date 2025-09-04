using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    FishControls fishControls;

    public Vector2 movementInput;
    public Vector2 cameraInput;

    public float cameraInputX;
    public float cameraInputY;

    public float verticalInput;
    public float horizontalInput;

    public bool swimmingUp;
    public bool swimmingDown;

    public FishMovement fishMovement;

    public GameObject fishUI;

    public GameObject endgamePanel;


    [HideInInspector]
    public bool[] fishFlags;

    private void Start()
    {
        fishMovement = GetComponent<FishMovement>();
        fishFlags = new bool[1];
        fishFlags[0] = true;
    }

    private void OnEnable()
    {
        if (fishControls == null)
        {
            fishControls = new FishControls();

            fishControls.FishMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            fishControls.FishMovement.Camera.performed += i => cameraInput = i.ReadValue<Vector2>();


            fishControls.FishMovement.Clownfish.performed += i =>
            {
                for (int j = 0; j < fishFlags.Length; j++)
                {
                    fishFlags[j] = false;
                }
                fishFlags[0] = true;
            };
            fishControls.FishMovement.Angelfish.performed += i => {
                if (fishFlags.Length > 1) { 
                    for (int j = 0; j < fishFlags.Length; j++)
                    {
                        fishFlags[j] = false;
                    }
                    fishFlags[1] = true;
                }
            };
            fishControls.FishMovement.Bass.performed += i =>
            {
                if (fishFlags.Length > 2)
                {
                    for (int j = 0; j < fishFlags.Length; j++)
                    {
                        fishFlags[j] = false;
                    }
                    fishFlags[2] = true;
                }

            }; 
            fishControls.FishMovement.SeaTurtle.performed += i =>
            {
                if (fishFlags.Length > 3)
                {
                    for (int j = 0; j < fishFlags.Length; j++)
                    {
                        fishFlags[j] = false;
                    }
                    fishFlags[3] = true;
                }
            };
            fishControls.FishMovement.Shark.performed += i =>
            {
                if (fishFlags.Length > 4)
                {
                    for (int j = 0; j < fishFlags.Length; j++)
                    {
                        fishFlags[j] = false;
                    }
                    fishFlags[4] = true;
                }
            };
        }

        fishControls.Enable();
    }

    private void OnDisable()
    {
        fishControls.Disable();
    }

    public void HandleAllInputs()
    {
        if (StartUI.gameStarted && !Cursor.visible)
            HandleMovementInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        cameraInputX = cameraInput.x;
        cameraInputY = cameraInput.y;
    } 

    public void AddNewFish(Sprite fishSprite)
    {
        fishFlags = new bool[fishFlags.Length + 1];

        fishUI.transform.GetChild(fishFlags.Length - 1).gameObject.SetActive(true);
        fishUI.transform.GetChild(fishFlags.Length - 1).GetComponent<Image>().sprite = fishSprite;
        for (int i=0; i < fishFlags.Length-1; i++)
        {
            fishFlags[i] = false;
        }
        fishFlags[fishFlags.Length - 1] = true;

        if (fishFlags.Length == 5)
        {
            endgamePanel.GetComponent<Endgame>().EndMinigame();
        }
    }
}
