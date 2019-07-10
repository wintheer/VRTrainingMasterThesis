using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class TightenAction : MonoBehaviour {

    private GameObject screwDriver, screwDriverPivot;

    private float interactionDistance = 0.05f;
    private float initialDistance, previousRotation;
    public float totalMoved { get; set; }
    public float totalMovement { get; set; }
    private float totalRotated = 0;
    private VibrateController vibrate;
    private bool shouldCalculateDistance = true;
    private int vibrationCount = 0;
    private bool doneRotating;
    private AudioSource audioData;
    private bool hasSentVibration;

    public float rotationDegrees = 360;

    public enum TightenOrLoosen { Tighten, Loosen }

    public TightenOrLoosen tightenOrLoosen;

    private string whichHand;

    public bool shouldActivate { get; set; }

    public bool doneTightening { get; set; }

    public GameObject destinationEmpty; // Drag and drop an empty object to the place it needs to stop rotating towards.

    void Start() {
        vibrate = transform.Find("/Vibration").GetComponent<VibrateController>();
        screwDriver = GameObject.Find("/TorquePlaceholder/Torque");
        screwDriverPivot = screwDriver.transform.GetChild(0).gameObject;
        audioData = GetComponent<AudioSource>();
    }

    void Update() {
        if (screwDriver.transform.parent.parent != null) {
            whichHand = screwDriver.transform.parent.parent.name;
        }

        float distanceToPivot = Vector3.Distance(transform.position, screwDriverPivot.transform.position);
        float distanceToDestination = Vector3.Distance(destinationEmpty.transform.position, transform.position);
        

        //print("Distance is: " + distanceToPivot);

        if (shouldActivate) {

            if (shouldCalculateDistance) {
                initialDistance = Vector3.Distance(transform.position, destinationEmpty.transform.position);
                shouldCalculateDistance = false;
            }

            // Check if the pivot point of the screwdriver is in proximity of the screw
            if (distanceToPivot <= interactionDistance) {
                float rotatedAmount = screwDriverPivot.transform.eulerAngles.z - previousRotation;
                if ((distanceToDestination > 0.001f && rotatedAmount < -0.5f) || (distanceToDestination > 0.001f && rotatedAmount > 0.5f)) {

                    Vector3 movement = new Vector3(initialDistance / 180, 0, 0);

                    if (tightenOrLoosen == TightenOrLoosen.Loosen) {
                        rotatedAmount = -rotatedAmount;
                        movement = -movement;
                    }
                    transform.Rotate(rotatedAmount, 0, 0, Space.Self);
                    totalMovement += Mathf.Abs(movement.magnitude);
                    totalMoved = Mathf.Abs (totalMovement / (initialDistance - 0.005f));
                    
                    transform.Translate(movement);
                    totalRotated += rotatedAmount;
                    
                } else if (distanceToDestination <= 0.005f) {
                    doneRotating = true;
                }

                if (doneRotating) {
                    if (tightenOrLoosen == TightenOrLoosen.Tighten) {
                        if (whichHand.Equals("RightHand")) {
                            if (!hasSentVibration) {
                                vibrate.Vibrate(XRNode.RightHand, 1, 10);
                                hasSentVibration = true;
                            }
                        } else {
                            if (!hasSentVibration) {
                                vibrate.Vibrate(XRNode.LeftHand, 1, 10);
                                hasSentVibration = true;
                            }
                        }
                    }
                    
                    doneTightening = true;
                    if (tightenOrLoosen == TightenOrLoosen.Tighten) {
                        if (!audioData.isPlaying) {
                            audioData.Play();
                        }
                    }
                }
                previousRotation = screwDriverPivot.transform.eulerAngles.z;
            }
        }
    }
}
