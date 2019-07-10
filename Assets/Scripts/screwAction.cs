using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class screwAction : MonoBehaviour
{
    public HandAnimation leftHand;
    public HandAnimation rightHand;
    public float distanceToHand = 0.1f;
    public GameObject endPos;
    private bool performAction = false;
    private SceneLevelManager manager;
    
    // Start is called before the first frame update
    void Start()
    {
        manager = FindObjectOfType<SceneLevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        print("Screwaction is on object: " + name);
        float distLeft = Vector3.Distance(leftHand.transform.position, transform.position);
        float distRight = Vector3.Distance(rightHand.transform.position, transform.position);
        //print("Print: "+distRight);
        if ((leftHand.isGrabbing == true && distLeft <= distanceToHand) || (rightHand.isGrabbing == true && distRight <= distanceToHand)) {
            performAction = true;
            // Green color glow
        }
        if (endPos.GetComponent<SimpleAnimation>().done) {
            // Stop glow
            performAction = false;
            //PersistenceManager.Instance.valveAmount += 1;
            if(this.tag == "ShiftItem"){
                manager.GetComponent<SceneLevelManager>().LoadScene(manager.GetComponent<SceneLevelManager>().getCurrentSceneNumber()+1);
               // manager.LoadScene(manager.getCurrentSceneNumber()+1);
            }
        }
        if (performAction == true) {
            endPos.GetComponent<SimpleAnimation>().Animate();
        }
    }
}
