using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class VibrateController : MonoBehaviour
{
    public static VibrateController Instance;
    private bool doneVibrating;

    void Awake() {
        Instance = this;
    }

    /**
     * Function is only meant to be called once per controller!!
     **/
    public void Vibrate(XRNode whichController, float intensity, int vibrationTimes) {
        InputDevice controller;
        if (whichController == XRNode.LeftHand ||  whichController == XRNode.RightHand) {
            controller = InputDevices.GetDeviceAtXRNode(whichController);
            StartCoroutine(doVibrations(controller, intensity, vibrationTimes));

        } else {
            throw new System.Exception("Can only use controllers for this method!");
        }
        if (doneVibrating) {
            StopCoroutine(doVibrations(controller, intensity, vibrationTimes));
        }
    }

    private IEnumerator doVibrations(InputDevice controller, float intensity, int duration) {
        int counter = 0;
        while (counter < duration) {
            yield return new WaitForSeconds(0.01f);
            controller.SendHapticImpulse(0, intensity);
            counter ++;
        }
        counter = 0;
        doneVibrating = true;
    }
}
