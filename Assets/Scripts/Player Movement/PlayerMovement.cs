using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour {

    [Header("Player Movement")]
    public float playerSpeed = 1f;
    public float drag; //this value stops us from sliding around too much. it could be turned off while the player isn't on the ground, but since we're underwater some drag would also make sense
    public float jumpForce;
    public float jumpCooldown;
    //public float airMultiplier;

    public Slider speedSlider;

    [Header("Ground Check")]
    public float playerHeight;
    //public LayerMask groundMask;
    //bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    RaycastHit slopeHit;
    bool exitingSlope;

    [Header("Camera")]
    public Transform orientation;


    bool flying = false;
    float doubleTapTimer = 1f;
    float elapsedTime = 0f;
    int pressCount;


    float horizontalInput;
    float verticalInput;
    float speedChange;
    Vector3 moveDirection;

    Rigidbody rb;

    private void Start() {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.drag = drag; //since this doesn't currently change, we can set it here
    }

    void Update() {
        GetInput();
    }

    void FixedUpdate() {
        MovePlayer();
        SpeedControl();
    }

    private void GetInput() {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");


        //code from https://forum.unity.com/threads/how-to-recognise-the-space-bar-tapped-pressed-twice-within-a-second.406116/
        if ((Input.GetKeyDown(KeyCode.Space) && !flying) || (flying && Input.GetKey(KeyCode.Space))) {
            Jump();

            //only increase the count the first time the key is pressed, not if it's held
            if (Input.GetKeyDown(KeyCode.Space)) {
                pressCount++;
            }
        }

        if (pressCount > 0) {
            elapsedTime += Time.deltaTime;
            if (elapsedTime > doubleTapTimer) {
                resetPressTimer();
            }
            else if (pressCount == 2) {
                //the player has just double pressed space
                flying = !flying;
                rb.useGravity = !flying; //gravity is true when flying is false

                resetPressTimer();
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && flying) {
            Lower();
        }

        // speed control
        speedChange = Input.GetAxis("Mouse ScrollWheel");
        speedSlider.value += speedChange * 10;
    }

    void resetPressTimer() {
        pressCount = 0;
        elapsedTime = 0;
        exitingSlope = false;
    }

    private void MovePlayer() {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        //on slope
        if (OnSlope() && !exitingSlope) {
            rb.AddForce(GetSlopeMoveDirection() * playerSpeed, ForceMode.Force);

            if (rb.velocity.y > 0) {
                rb.AddForce(Vector3.down * 20f, ForceMode.Force);
            }
        }
        else {
            rb.AddForce(moveDirection.normalized * playerSpeed, ForceMode.Force);
        }

        if (!flying) {
            rb.useGravity = !OnSlope();
        }

    }

    //Ensure the velocity never exceeds the player's speed
    void SpeedControl() {
        //limit speed on slope
        if (OnSlope()) {
            if (rb.velocity.magnitude > playerSpeed) {
                rb.velocity = rb.velocity.normalized * playerSpeed;
            }
        }
        //limit speed on ground/in water
        else {
            Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVelocity.magnitude > playerSpeed) {
                Vector3 limitedVelocity = flatVelocity.normalized * playerSpeed;
                rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
            }
        }
    }

    void Jump() {
        exitingSlope = true;

        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void Lower() {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(-1 * transform.up * jumpForce, ForceMode.Impulse);
    }

    bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f)) {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    Vector3 GetSlopeMoveDirection() {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
}
