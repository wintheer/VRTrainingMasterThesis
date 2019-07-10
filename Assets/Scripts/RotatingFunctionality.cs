using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;

public class RotatingFunctionality : MonoBehaviour
{
    public HandAnimation leftHandAnimation;
    public HandAnimation rightHandAnimation;
    public float distanceToHand = 0.1f;
    private bool performRotation = false;
    private SceneLevelManager manager;
    private VibrateController vibrate;
    public float speed = 0.5f;
    public GameObject objectToRotate;

    private bool timeHasStarted;
    private GameObject leftHandThumbTip, rightHandThumbTip;
    private GameObject leftButtonObj, rightButtonObj, diaphragmDestination, diaphragm;
    private AudioSource audioData;
    public Stopwatch time;

    //Kasper code starts here
    public GameObject targetObject;
    private Quaternion initRot;
    private Vector3 targetRot;

    public bool animationHasFinished { get; set; }
    public bool hasBeenRotated { get; set; }

    private Transform trans;

    void Start() {
        vibrate = transform.Find("/Vibration").GetComponent<VibrateController>();
        time = new Stopwatch();
        manager = FindObjectOfType<SceneLevelManager>();
        initRot = this.transform.rotation;
        trans = targetObject.transform;

        leftHandThumbTip = GameObject.Find("/CameraPlaceholder/LeftHand/LeftHandSkinnedMesh/hands:l_hand_world/hands:b_l_hand/hands:b_l_thumb1" +
            "/hands:b_l_thumb2/hands:b_l_thumb3/hands:b_l_thumb_ignore");

        rightHandThumbTip = GameObject.Find("/CameraPlaceholder/RightHand/RightHandSkinnedMesh/hands:l_hand_world/hands:b_l_hand/hands:b_l_thumb1" +
            "/hands:b_l_thumb2/hands:b_l_thumb3/hands:b_l_thumb_ignore");

        leftButtonObj = GameObject.Find("LeftButtonPos");
        rightButtonObj = GameObject.Find("RightButtonPos");
        diaphragmDestination = GameObject.Find("DiaphragmPosition");
        diaphragm = GameObject.Find("/DetailedPump2/Diaphragm");
        audioData = GetComponent<AudioSource>();
    }

    void Update() {
        float distLeft = Vector3.Distance(leftHandAnimation.transform.position, transform.position);
        float distRight = Vector3.Distance(rightHandAnimation.transform.position, transform.position);

        if ((leftHandAnimation.isGrabbing == true && distLeft <= distanceToHand) && !hasBeenRotated 
            || (rightHandAnimation.isGrabbing == true && distRight <= distanceToHand) && !hasBeenRotated) {
            performRotation = true;
        }

        if (performRotation == true) {
            //print("Rotation: " + objectToRotate.transform.rotation.x);
            if (objectToRotate.transform.rotation.x <= 0.999f) {
                hasBeenRotated = true;
                objectToRotate.transform.RotateAround(trans.position, Vector3.left, speed * Time.deltaTime);
            }  
        }

        // Get the positions of each thumb to see if the buttons are pressed
        if (hasBeenRotated) {
            if (Vector3.Distance(leftHandThumbTip.transform.position, leftButtonObj.transform.position) < 0.05f
                && Vector3.Distance(rightHandThumbTip.transform.position, rightButtonObj.transform.position) < 0.05f) {
                // Each finger is near left and right button!

                if (!timeHasStarted) {
                    time.Start();
                    timeHasStarted = true;
                }

                if (time.ElapsedMilliseconds >= 1800) {
                    // Move the diaphragm!
                    float step = 1.0f * Time.deltaTime;
                    if (!animationHasFinished) {
                        diaphragm.transform.position = Vector3.MoveTowards(diaphragm.transform.position, diaphragmDestination.transform.position, step);
                        if (!audioData.isPlaying) {
                            audioData.Play();
                        }
                        // if the objects are very close, stop the animation!
                        if (Vector3.Distance(diaphragm.transform.position, diaphragmDestination.transform.position) < 0.001f) {
                            // Send a haptic pulse to let the user know that the action is finished
                            vibrate.Vibrate(XRNode.RightHand, 0.5f, 3);
                            vibrate.Vibrate(XRNode.LeftHand, 0.5f, 3);
                            animationHasFinished = true;
                        }
                    }
                }
            }
            else {
                time.Reset();
                timeHasStarted = false;
            }
        }
    }
}

