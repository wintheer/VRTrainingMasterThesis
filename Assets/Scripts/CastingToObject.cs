using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastingToObject : MonoBehaviour
{
    public static string selectedObject, handObject;
    public string internalObject, internalHandObject;
    public static bool isCloseEnough = false;
    public RaycastHit theObject;
    public float grabbingDistance = 0.2f;
    
    void Start() {
        handObject = gameObject.name;
        internalHandObject = gameObject.name;
    }

    void Update()
    {
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out theObject)) {
            if (GameObject.Find(theObject.transform.gameObject.name).tag == "Grab") {
                selectedObject = theObject.transform.gameObject.name;
                internalObject = theObject.transform.gameObject.name;

                // This condition is true if he distance is below some threshold
                if (Vector3.Distance(GameObject.Find(internalHandObject).transform.position, 
                    GameObject.Find(selectedObject).transform.position) <= grabbingDistance) {
                    isCloseEnough = true;
                } else {
                    isCloseEnough = false;
                }
            }
        }
    }
}
