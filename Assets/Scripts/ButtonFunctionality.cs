using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ButtonFunctionality : MonoBehaviour
{
    public HandAnimation leftHand;
    public HandAnimation rightHand;
    public GameObject otherButton;
    public float distanceToHand = 0.6f;
    private SceneLevelManager manager;
    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<SceneLevelManager>();
    }

    // Update is called once per frame
    void Update() {
        print("Bad script on: " + name);
        float distLeft = Vector3.Distance(leftHand.transform.position, transform.position);
        float distRight = Vector3.Distance(rightHand.transform.position, otherButton.transform.position);
        if ((leftHand.isGrabbing == true && distLeft <= distanceToHand) && (rightHand.isGrabbing == true && distRight <= distanceToHand)) {
            // The buttons are pressed simultaneously -> give feedback to user
            
            manager.GetComponent<SceneLevelManager>().LoadScene(manager.GetComponent<SceneLevelManager>().getCurrentSceneNumber()+1);
        }
    }

}
