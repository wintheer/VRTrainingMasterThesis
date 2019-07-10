using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedFlash : MonoBehaviour
{
    public GameObject selectedObject, castingObject;
    public int redCol, greenCol, blueCol;
    public bool lookingAtObject = false;
    public bool flashingIn = true;
    public bool startedFlashing = false;
    public float distanceBetweenThisAndObject = 0.2f;
    public string interactionTag = "Grab";

    void Update() {
        checkIfNear();
        if (lookingAtObject == true) {
            selectedObject.GetComponent<Renderer>().material.color = new Color32((byte)redCol, (byte)greenCol, (byte)blueCol, 255); // Red, green, blue, alpha values
        }
    }

    void checkIfNear() {
        selectedObject = GameObject.Find(CastingToObject.selectedObject);

        if (selectedObject != null) {
            // The object can be interacted with because it has the right tag!
            if (selectedObject.tag == interactionTag) {
                //Debug.Log("Selected object is: " + selectedObject.name);
                castingObject = GameObject.Find(CastingToObject.handObject);
                Vector3 selectedObjectPos = selectedObject.transform.position;
                Vector3 handObjectPos = castingObject.transform.position;
                float distance = Vector3.Distance(selectedObjectPos, handObjectPos);

                // If the distance is small enough the object can be moved with the hand
                if (CastingToObject.isCloseEnough) {
                    //Debug.Log("Distance between hand and " + selectedObject.name + " is " + distance);
                    lookingAtObject = true;
                    if (startedFlashing == false) {
                        startedFlashing = true;
                        StartCoroutine(flashObject());
                    }
                } else { 
                    lookingAtObject = false;
                    startedFlashing = false;
                    StopCoroutine(flashObject());
                    selectedObject.GetComponent<Renderer>().material.color = new Color32(255, 255, 255, 255); // Reset colors of object!
                }
            }
        }
    }

    IEnumerator flashObject() {
        while (lookingAtObject) {
            yield return new WaitForSeconds(0.05f);

            if (flashingIn == true) {
                //It's time to turn the color to something darker!
                if (blueCol <= 30) {
                    Debug.Log("Time to make the object brighter!");
                    flashingIn = false;
                } else {
                    redCol -= 25;
                    greenCol -= 25;
                    blueCol -= 25;
                }
            }

            if (flashingIn == false) {
                // It's time to turn the color to something brighter!
                if (blueCol >= 250) {
                    flashingIn = true;
                    Debug.Log("Time to make the object darker!");
                } else {
                    redCol += 25;
                    greenCol += 25;
                    blueCol += 25;
                }
            }
        }
    }
}
