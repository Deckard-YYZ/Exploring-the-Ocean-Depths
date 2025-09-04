using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    InputManager inputManager;


    Vector3 moveDirection;
    Transform cameraObject;
    Rigidbody fishRigidbody;

    public float movementSpeed = 12f;
    public float rotationSpeed = 15;

    public float swimUpSpeed = 12f;

    private string lookDirection = "";



    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        fishRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
    }

    public void HandleAllMovement()
    {
        if (StartUI.gameStarted)
        {
            HandleMovement();
            HandleRotation();
            HandleSwimmingUp();
            HandleSwimmingDown();
        }
    }

    private void HandleMovement()
    {
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inputManager.horizontalInput;
        moveDirection.Normalize();
        moveDirection.y = 0f;
        moveDirection *= movementSpeed;

        Vector3 movementVelocity = moveDirection;
        fishRigidbody.velocity = movementVelocity;
    }

    private void HandleRotation()
    {
        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizontalInput;
        targetDirection.Normalize();
        targetDirection.y = 0f;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        if (lookDirection == "UP")
        {
            Vector3 targetRotationEuler = targetRotation.eulerAngles;
            targetRotationEuler -= new Vector3(30f, 0f, 0f);
            if (targetDirection == transform.forward)
            {
                targetRotationEuler.x = Mathf.Max(targetRotationEuler.x, 270f);
            }

            targetRotation = Quaternion.Euler(targetRotationEuler);
        }
        else if (lookDirection == "DOWN")
        {
            Vector3 targetRotationEuler = targetRotation.eulerAngles;
            targetRotationEuler += new Vector3(30f, 0f, 0f);
            targetRotationEuler.x = Mathf.Min(targetRotationEuler.x, 90f);
            targetRotation = Quaternion.Euler(targetRotationEuler);
        }


        Quaternion fishRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = fishRotation;
    }

    public void HandleSwimmingUp()
    {
        if (Input.GetKey(KeyCode.Space) && lookDirection != "DOWN") {
            Vector3 upMovement = Vector3.up * swimUpSpeed * Time.deltaTime; 
            fishRigidbody.MovePosition(transform.position + upMovement);

            lookDirection = "UP";
        }
        else if (lookDirection != "DOWN")
        {
            lookDirection = "";
        }
    }

    public void HandleSwimmingDown()
    {
        if (Input.GetKey(KeyCode.LeftShift) && lookDirection != "UP")
        {
            Vector3 downMovement = Vector3.down * swimUpSpeed * Time.deltaTime;
            fishRigidbody.MovePosition(transform.position + downMovement);
            lookDirection = "DOWN";

        }
        else if (lookDirection != "UP")

        {
            lookDirection = "";

        }
    }

    public void setSpeed(float speed)
    {
        movementSpeed = speed;
    }
}
