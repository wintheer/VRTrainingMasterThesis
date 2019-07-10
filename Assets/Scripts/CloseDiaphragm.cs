using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.XR;

public class CloseDiaphragm : MonoBehaviour {

    private bool timeHasStarted;
    private VibrateController vibrate;
    private GameObject leftHandThumbTip, rightHandThumbTip;
    private GameObject leftButtonObj, rightButtonObj, diaphragmDestination, diaphragm;
    private AudioSource audioData;

    public Stopwatch time;

    public bool animationHasFinished { get; set; }

    void Start() {
        vibrate = transform.Find("/Vibration").GetComponent<VibrateController>();
        time = new Stopwatch();
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
            if (time != null) {
                time.Reset();
                timeHasStarted = false;
            }
        }
    }
}
