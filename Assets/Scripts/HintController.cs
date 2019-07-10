using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintController : MonoBehaviour
{
    GameObject hint1;
    GameObject leftHand;
    // Start is called before the first frame update
    void Start()
    {
        leftHand = GameObject.Find("Hand");
        hint1 = GameObject.Find("Hint1");
    }

    // Update is called once per frame
    void Update()
    {
        hintController(hint1);
    }
    void hintController(GameObject hint){
        float distLeftHand = Vector3.Distance(leftHand.transform.position, hint.transform.position);
        print(distLeftHand);
        if(distLeftHand < 0.3f){
            hint.SetActive(false);
        }
        else{
            hint.SetActive(true);
        }
    }
}
