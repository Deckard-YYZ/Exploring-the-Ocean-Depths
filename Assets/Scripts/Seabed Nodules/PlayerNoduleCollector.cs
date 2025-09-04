using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNoduleCollector : MonoBehaviour {
    public int numNodulesCollected;

    public UIController UIcontroller;
    public GameObject SedimentCloud;


    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Nodule") {
            
            //collect the nodule
            other.gameObject.SetActive(false);
            numNodulesCollected++;
            UIcontroller.UpdateCount(numNodulesCollected);

            //display the dust cloud in its place
            Instantiate(SedimentCloud, transform.position, transform.rotation);


            if (numNodulesCollected % 6 == 0) { //there's 111 nodules in total
                UIcontroller.ShowFunFact(numNodulesCollected / 6);
            }
        }
    }
}
