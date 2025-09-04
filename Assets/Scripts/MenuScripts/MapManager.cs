using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject mapCanvas;
    private bool mapOn;
    // Start is called before the first frame update
    void Start()
    {
        mapCanvas.SetActive(false);
        mapOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            mapCanvas.SetActive(!mapOn);
            mapOn = !mapOn;
        }
    }
}
