using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class RadialProgress : RadialFill
{
    // Event to invoke when the progress bar fills up
    private UnityEvent onProgressComplete;
    private GameObject topPlug, bottomPlug, diaphragm, screw1, screw2, screw3, screw4, extendDiaphragm, retractDiaphragm, canvas;
    private float distanceToRightHand, distanceToLeftHand;
    private HandAnimation LeftHand, RightHand;
    //private enum HandType { Left, Right }
    private List<GameObject> objects;
    private float progressed;
    private Vector3 distanceLeft;
    private float interactionDistance = 0.1f;

    // Create a property to handle the slider's value
    public new float CurrentValue {
        get {
            return base.CurrentValue;
        }
        set {
            // If the value exceeds the max fill, invoke the completion function
            if (value > maxValue)
                onProgressComplete.Invoke();

            // Remove any overfill (i.e. 105% fill -> 5% fill)
            base.CurrentValue = value % maxValue;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if (onProgressComplete == null){
            onProgressComplete = new UnityEvent();
            onProgressComplete.AddListener(OnProgressComplete);
        }
        FindGameObjects();
        objects = new List<GameObject>() { topPlug, bottomPlug, diaphragm, screw1, screw2, screw3, screw4 };
        if (GameObject.Find("FrontLid_trigger") != null) {
            extendDiaphragm = GameObject.Find("FrontLid_trigger");
            objects.Add(extendDiaphragm);
        }
        if (GameObject.Find("DiaphragmMovement") != null) {
            retractDiaphragm = GameObject.Find("DiaphragmMovement");
            objects.Add(retractDiaphragm);
        }
        canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject o in objects){
            if (o.GetComponent<SnapToPlace>() != null && (LeftHand.isGrabbing || RightHand.isGrabbing) && o.GetComponent<SnapToPlace>().ShouldActivate == true){
                distanceToLeftHand = Vector3.Distance(o.transform.position, LeftHand.transform.position);
                distanceToRightHand = Vector3.Distance(o.transform.position, RightHand.transform.position);
                if(distanceToLeftHand <= interactionDistance || distanceToRightHand <= interactionDistance){
                    if ((o.GetComponent<HandLoosen>() != null && o.GetComponent<HandLoosen>().shouldActivate) ||
                        (o.GetComponent<TightenAction>() != null && o.GetComponent<TightenAction>().shouldActivate)) {
                        // Begin to show progress bar
                        canvas.SetActive(true);
                        progressAction(o);
                        if (o.transform.name.Equals("Diaphragm") || o.transform.name.Equals("Screw1") ||
                        o.transform.name.Equals("Screw2") || o.transform.name.Equals("Screw3") ||
                        o.transform.name.Equals("Screw4")) {
                            // Position at the object's position
                            transform.position = new Vector3(o.transform.position.x, o.transform.position.y, o.transform.position.z);
                        } 
                        else {
                            // Position on the top of object's position
                            transform.position = new Vector3(o.transform.position.x, o.transform.position.y + 0.15f, o.transform.position.z);
                        }
                    }
                    if ((o.GetComponent<HandLoosen>() != null && o.GetComponent<HandLoosen>().doneRotating) ||
                (o.GetComponent<TightenAction>() != null && o.GetComponent<TightenAction>().doneTightening)) {
                        // Stop showing progress bar
                        canvas.SetActive(false);
                    }
                }
            }
            if ((o.GetComponent<RotatingFunctionality>() != null && o.GetComponent<RotatingFunctionality>().time.IsRunning) ||
                (o.GetComponent<CloseDiaphragm>() != null && o.GetComponent<CloseDiaphragm>().time.IsRunning)) {
                canvas.SetActive(true);
                progressAction(o);
                transform.position = new Vector3(o.transform.position.x, o.transform.position.y + 0.15f, o.transform.position.z);
            }
            if ((o.GetComponent<RotatingFunctionality>() != null && o.GetComponent<RotatingFunctionality>().animationHasFinished) ||
                (o.GetComponent<CloseDiaphragm>() != null && o.GetComponent<CloseDiaphragm>().animationHasFinished)) {
                canvas.SetActive(false);
            }
        }
    }
    void OnProgressComplete() {
        //Debug.Log("Progress Complete");
    }
    void FindGameObjects(){
        LeftHand = GameObject.Find("/CameraPlaceholder/LeftHand").GetComponent<HandAnimation>();
        RightHand = GameObject.Find("/CameraPlaceholder/RightHand").GetComponent<HandAnimation>();
        topPlug = GameObject.Find("TopPlug");
        bottomPlug = GameObject.Find("BottomPlug");
        diaphragm = GameObject.Find("Diaphragm");
        screw1 = GameObject.Find("Screw1");
        screw2 = GameObject.Find("Screw2");
        screw3 = GameObject.Find("Screw3");
        screw4 = GameObject.Find("Screw4");
        canvas = GameObject.Find("/ProgressPlaceholder/Canvas");
    }
    void progressAction(GameObject ob){
        if(ob.transform.name.Equals("FrontLid_trigger")) {
            float timePassedInPercentage = (ob.GetComponent<RotatingFunctionality>().time.ElapsedMilliseconds) / 1800f;
            if (timePassedInPercentage > 1f) {
                CurrentValue = 1;

            } 
            else {
                CurrentValue = timePassedInPercentage;
            }
        }
        if (ob.transform.name.Equals("DiaphragmMovement")) {
            float timePassedInPercentage = (ob.GetComponent<CloseDiaphragm>().time.ElapsedMilliseconds) / 1800f;
            if (timePassedInPercentage > 1f) {
                CurrentValue = 1;

            } else {
                CurrentValue = timePassedInPercentage;
            }
        }
        if (ob.transform.name.Equals("TopPlug") || ob.transform.name.Equals("BottomPlug") || ob.transform.name.Equals("Diaphragm")){
            CurrentValue = ob.GetComponent<HandLoosen>().TotalRotation();
        }
        if (ob.transform.name.Equals("Screw1") || ob.transform.name.Equals("Screw2") ||
            ob.transform.name.Equals("Screw3") || ob.transform.name.Equals("Screw4")) {
            if (ob.GetComponent<TightenAction>().totalMoved > 1f) {
                CurrentValue = 1;

            } else {
                CurrentValue = ob.GetComponent<TightenAction>().totalMoved;
            }
        }
    }
}
