using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class CompassTest : MonoBehaviour
{
    public Transform player;
    public GameObject CompassPoint;

    private Vector3 playerPos;
    private double playerRotY;
    private double playerRotLow;
    private double playerRotHigh;
    private double fov = 90;
    private double compassWidth = 540;
    
    private Vector3 objPos;
    
    private GameObject[] poiList;
    private GameObject[] pointers;
    
    private Vector3 diff;
    private double angle;
    private int quadrant;
    
    private GameObject tempObj;
    
    
    // Start is called before the first frame update
    void Start()
    {
        poiList = GameObject.FindGameObjectsWithTag("poi");
        pointers = new GameObject[poiList.Length];
        for (int i = 0; i < poiList.Length; i++)
        {
            //tempObj = new GameObject();
            tempObj = Instantiate(CompassPoint);
            //tempObj.AddComponent<Image>();
            tempObj.transform.position = new Vector3(0,0,0);
            tempObj.transform.SetParent(this.transform);
            tempObj.GetComponent<Image>().rectTransform.anchorMin = new Vector2((float)0.5, 1);
            tempObj.GetComponent<Image>().rectTransform.anchorMax = new Vector2((float)0.5, 1);
            tempObj.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 20);
            tempObj.GetComponent<Image>().rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 20);
            tempObj.SetActive(false);
            pointers[i] = tempObj;
        }
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = player.position;
        playerRotY = player.eulerAngles.y;
        playerRotLow = playerRotY - (fov / 2);
        playerRotHigh = playerRotY + (fov / 2);
        
        for(int i = 0; i<poiList.Length; i++)
        {
            GameObject obj = poiList[i];
            objPos = obj.transform.position;
            
            diff = (new Vector3(objPos.x, 0, objPos.z) - new Vector3(playerPos.x, 0, playerPos.z));
            
            
            //Debug.Log(diff);
            if (diff is { x: <= 0, z: <= 0 })
            {
                angle = Math.Atan(diff.x / diff.z)*(180 / Math.PI);
                angle += 180;
            }
            else if (diff is { x: <= 0, z: >= 0 })
            {
                angle = Math.Atan(diff.z / diff.x)*(180 / Math.PI);
                angle = angle * -1 + 270;
            }
            else if (diff is { x: >= 0, z: <= 0 })
            {
                angle = Math.Atan(diff.z / diff.x)*(180 / Math.PI);
                angle = angle * -1 + 90;
            }
            else
            {
                angle = Math.Atan(diff.x / diff.z)*(180 / Math.PI);
            }
            //Debug.Log((angle, playerRotY, playerRotLow, playerRotHigh));
            
            if (angle > playerRotLow && angle < playerRotHigh)
            {
                pointers[i].SetActive(true);
                pointers[i].transform.localPosition =  new Vector3((float)((compassWidth - ((playerRotHigh - angle) * (compassWidth / fov))-(compassWidth/2))), (float)180, 0);
            }
            else if (playerRotHigh > 360 && (angle > playerRotLow-360 && angle < playerRotHigh-360))
            {
                pointers[i].SetActive(true);
                pointers[i].transform.localPosition =  new Vector3((float)((compassWidth - ((playerRotHigh-360 - angle) * (compassWidth / fov))-(compassWidth/2))), (float)180, 0);
            }
            else if (playerRotLow < 0 && angle > playerRotLow+360 && angle < playerRotHigh+360)
            {
                pointers[i].SetActive(true);
                pointers[i].transform.localPosition =  new Vector3((float)((compassWidth - ((playerRotHigh+360 - angle) * (compassWidth / fov))-(compassWidth/2))), (float)180, 0);
            }
            else{
                pointers[i].SetActive(false);
            }
        }
    }
}
