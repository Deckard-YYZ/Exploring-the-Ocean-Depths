using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectiblePickup : MonoBehaviour
{
    public InputManager inputs;
    public GameObject player;
    public GameObject fishPrefab;
    public Sprite fishIcon;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FishPlayer")
        {
            inputs.AddNewFish(fishIcon);
            GameObject fishPlayer = Instantiate(fishPrefab, player.transform.position, player.transform.rotation);
            fishPlayer.transform.SetParent(player.transform);
            Destroy(gameObject);
        }
    }
}
