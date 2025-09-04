using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedController : MonoBehaviour
{
    public PlayerMovement playerMovement;

    public void UpdateSpeed(System.Single speed) {
        playerMovement.playerSpeed = speed;
    }
}
