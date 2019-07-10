using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1Steps : MonoBehaviour
{

    private GameObject backScrew;
    
    private SceneLevelManager slm;

    public void ScrewOff()
    {
        Debug.Log("screwing off");
        backScrew = GameObject.Find("Back_screw");
        backScrew.GetComponent<Rigidbody>().useGravity = true;
        backScrew.GetComponent<Rigidbody>().AddTorque(1, 0, 0);
        
        Destroy(this);
    }
}
