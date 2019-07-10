using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StepList : MonoBehaviour
{
    public GameObject steplistPosition;
    public int distance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Setting canvas position equal to position of specific gameobject in scene
        transform.position = steplistPosition.transform.position + steplistPosition.transform.forward * distance;
        //transform.position.y = steplistPosition.transform.position.y;
        //transform.rotation = cam.transform.rotation;
    }
}
