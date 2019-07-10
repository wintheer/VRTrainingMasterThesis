using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class SandboxProgressbar : RadialFill {
    // Event to invoke when the progress bar fills up
    private UnityEvent onProgressComplete;
    private GameObject lScrew1, lScrew2, lScrew3, lScrew4, tScrew1, tScrew2, tScrew3, tScrew4, valve, rocket, canvas;
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
    void Start() {
        if (onProgressComplete == null) {
            onProgressComplete = new UnityEvent();
            onProgressComplete.AddListener(OnProgressComplete);
        }
        FindGameObjects();
        objects = new List<GameObject>() { lScrew1, lScrew2, lScrew3, lScrew4, tScrew1, tScrew2, tScrew3, tScrew4, valve, rocket };
        canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        foreach (GameObject o in objects) {
            if (o.GetComponent<SnapToPlace>() != null && (LeftHand.isGrabbing || RightHand.isGrabbing) && o.GetComponent<SnapToPlace>().ShouldActivate == true) {
                distanceToLeftHand = Vector3.Distance(o.transform.position, LeftHand.transform.position);
                distanceToRightHand = Vector3.Distance(o.transform.position, RightHand.transform.position);
                if (distanceToLeftHand <= interactionDistance || distanceToRightHand <= interactionDistance) {
                    if (o.GetComponent<TightenAction>() != null && o.GetComponent<TightenAction>().shouldActivate) {
                        // Begin to show progress bar
                        canvas.SetActive(true);
                        progressAction(o);
                        if (o.transform.name.Equals("LScrew1") || o.transform.name.Equals("LScrew2") ||
                            o.transform.name.Equals("LScrew3") ||
                        o.transform.name.Equals("LScrew4") || o.transform.name.Equals("TScrew1") ||
                            o.transform.name.Equals("TScrew2") || o.transform.name.Equals("TScrew3") ||
                            o.transform.name.Equals("TScrew4")) {
                            // Position at the object's position
                            transform.position = new Vector3(o.transform.position.x, o.transform.position.y, o.transform.position.z);
                        }
                    }
                    if (o.GetComponent<TightenAction>() != null && o.GetComponent<TightenAction>().doneTightening) {
                        // Stop showing progress bar
                        canvas.SetActive(false);
                    }
                }
            }
            if (o.GetComponent<HandLoosen>() != null && (LeftHand.isGrabbing || RightHand.isGrabbing)) {
                distanceToLeftHand = Vector3.Distance(o.transform.position, LeftHand.transform.position);
                distanceToRightHand = Vector3.Distance(o.transform.position, RightHand.transform.position);
                if (distanceToLeftHand <= interactionDistance || distanceToRightHand <= interactionDistance) {
                    canvas.SetActive(true);
                    progressAction(o);
                    transform.position = new Vector3(o.transform.position.x, o.transform.position.y + 0.15f, o.transform.position.z);
                }
            }
            if (o.GetComponent<HandLoosen>() != null && o.GetComponent<HandLoosen>().doneRotating) {
                canvas.SetActive(false);
            }
            if (o.GetComponent<SandboxBehavior>() != null && o.GetComponent<SandboxBehavior>().stopwatch.IsRunning) {
                canvas.SetActive(true);
                progressAction(o);
                transform.position = new Vector3(o.transform.position.x, o.transform.position.y + 0.15f, o.transform.position.z);
            }
            if (o.GetComponent<SandboxBehavior>() != null && o.GetComponent<SandboxBehavior>().playedAnimation) {
                canvas.SetActive(false);
            }
        }
    }
    void OnProgressComplete() {
        //Debug.Log("Progress Complete");
    }
    void FindGameObjects() {
        LeftHand = GameObject.Find("/CameraPlaceholder/LeftHand").GetComponent<HandAnimation>();
        RightHand = GameObject.Find("/CameraPlaceholder/RightHand").GetComponent<HandAnimation>();
        lScrew1 = GameObject.Find("LScrew1");
        lScrew2 = GameObject.Find("LScrew2");
        lScrew3 = GameObject.Find("LScrew3");
        lScrew4 = GameObject.Find("LScrew4");
        tScrew1 = GameObject.Find("TScrew1");
        tScrew2 = GameObject.Find("TScrew2");
        tScrew3 = GameObject.Find("TScrew3");
        tScrew4 = GameObject.Find("TScrew4");
        valve = GameObject.Find("Valve/Valve");
        rocket = GameObject.Find("BehaviorScript");
        canvas = GameObject.Find("/ProgressPlaceholder/Canvas");
    }
    void progressAction(GameObject ob) {
        if (ob.transform.name.Equals("LScrew1") || ob.transform.name.Equals("LScrew2") ||
            ob.transform.name.Equals("LScrew3") || ob.transform.name.Equals("LScrew4") ||
            ob.transform.name.Equals("TScrew1") || ob.transform.name.Equals("TScrew2") ||
            ob.transform.name.Equals("TScrew3") || ob.transform.name.Equals("TScrew4")) {
            if (ob.GetComponent<TightenAction>().totalMoved > 1f) {
                CurrentValue = 1;

            } else {
                CurrentValue = ob.GetComponent<TightenAction>().totalMoved;
            }
        }
        if (ob.transform.name.Equals("Valve")) {
            if (ob.GetComponent<HandLoosen>().TotalRotation() >= 1f) {
                ob.GetComponent<HandLoosen>().resetRotation();
            } 
            else {
                CurrentValue = ob.GetComponent<HandLoosen>().TotalRotation();
            }
            CurrentValue = ob.GetComponent<HandLoosen>().TotalRotation();
        }
        if (ob.transform.name.Equals("BehaviorScript")) {
            CurrentValue = (ob.GetComponent<SandboxBehavior>().stopwatch.ElapsedMilliseconds) / 1800f;
        }
    }
}
