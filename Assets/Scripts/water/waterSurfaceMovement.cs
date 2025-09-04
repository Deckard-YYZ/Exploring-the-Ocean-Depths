using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waterSurfaceMovement : MonoBehaviour
{
    public Transform playerTrans;

    public float oceanSurfaceHeight;
    // Start is called before the first frame update

    private Vector3 _Position_with_no_y;
    

    // Update is called once per frame
    void Update()
    {
        
        _Position_with_no_y = playerTrans.position;
        transform.position = new Vector3(_Position_with_no_y.x, oceanSurfaceHeight, _Position_with_no_y.z);
        
    }
}
