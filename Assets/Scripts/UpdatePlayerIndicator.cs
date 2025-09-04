using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdatePlayerIndicator : MonoBehaviour
{
    public Transform player;
    private float topRightX = 49450;
    private float topRightZ = -27071;

    [SerializeField]
    private float playerRelativeXratio;
    [SerializeField]
    private float playerRelativeZratio;

    [SerializeField]
    private float playerRelativeXpos;
    [SerializeField]
    private float playerRelativeZpos;

    [SerializeField]
    private float worldImageLocationX;
    [SerializeField]
    private float worldImageLocationZ;

    private float widthWorld = 105439.0f;
    private float lengthWorld = 52715.0f;

   
    public float lengthWorldImage = 256.0f;
    
    public float widthWorldImage = 512.0f;


    // Update is called once per frame
    void Update()
    {
        // Update player location
        playerRelativeXpos = Mathf.Abs(player.position.x - topRightX);
        playerRelativeZpos = Mathf.Abs(player.position.z - topRightZ);

        playerRelativeXratio = playerRelativeXpos / widthWorld;
        playerRelativeZratio = playerRelativeZpos / lengthWorld;

        worldImageLocationX = 512 * playerRelativeXratio * -1;
        worldImageLocationZ = 256 * playerRelativeZratio * -1;

        transform.localPosition = new Vector3(worldImageLocationX, worldImageLocationZ, 0);
    }
}
