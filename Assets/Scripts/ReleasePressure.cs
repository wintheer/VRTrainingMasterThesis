using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class ReleasePressure : MonoBehaviour
{
    private GameObject leftGrip, rightGrip;
    private bool leftRotated, rightRotated;
    private HandAnimation leftHandAnim, rightHandAnim;
    private Quaternion leftOriginalOrientation, rightOriginalOrientation;
    private float prevLeftRot, prevRightRot;
    private ParticleSystem steam, gas;
    private float totalRotLeft, totalRotRight;
    private bool playedAnimation;
    private AudioSource audioData;
    private VibrateController vibrate;

    private enum HandType { Left, Right }

    public bool IsDone { get; set; }

    void Start() {
        initialiseComponents();
        audioData = GetComponent<AudioSource>();
    }

    void Update() {
        handleGrip(HandType.Left);
        handleGrip(HandType.Right);

        // Play animation if it has not already been played
        if (totalRotLeft > 30 && totalRotRight > 30) {
            if (!playedAnimation) {
                gas.Play();
                steam.Play();
                playedAnimation = true;
                vibrate.Vibrate(XRNode.RightHand, 0.5f, 10);
                vibrate.Vibrate(XRNode.LeftHand, 0.5f, 10);
                IsDone = true;
                if (!audioData.isPlaying) {
                    audioData.Play();
                }
            }
        }
    }

    private void handleGrip(HandType whichHand) {
        float distanceToHand;
        GameObject grip;
        bool isGrabbing;
        float rotatedAmount;

        if (whichHand == HandType.Left) {
            distanceToHand = Vector3.Distance(leftGrip.transform.position, leftHandAnim.transform.position);
            grip = leftGrip;
            isGrabbing = leftHandAnim.isGrabbing;
            rotatedAmount = leftHandAnim.transform.localEulerAngles.z - prevLeftRot;
            prevLeftRot = leftHandAnim.transform.localEulerAngles.z;
        } else {
            distanceToHand = Vector3.Distance(rightGrip.transform.position, rightHandAnim.transform.position);
            grip = rightGrip;
            isGrabbing = rightHandAnim.isGrabbing;
            rotatedAmount = rightHandAnim.transform.localEulerAngles.z - prevRightRot;
            prevRightRot = rightHandAnim.transform.localEulerAngles.z;
        }

        
        if (distanceToHand < 0.2f && isGrabbing) {
            // Allow it to rotate object!!
            if (rotatedAmount > 0) {
                grip.transform.Rotate(0, 0, -rotatedAmount, Space.Self);

                if (whichHand == HandType.Left) {
                    totalRotLeft += rotatedAmount;
                } else {
                    totalRotRight += rotatedAmount;
                }
            }
            
        } else {
            // Reset the orientation of the object
            if (whichHand == HandType.Left) {
                grip.transform.rotation = leftOriginalOrientation;
                totalRotLeft = 0;
            } else {
                grip.transform.rotation = rightOriginalOrientation;
                totalRotRight = 0;
            }   
        }
    }


    private void initialiseComponents() {
        leftGrip = transform.Find("/PressureReleaser/L_Grip").gameObject;
        rightGrip = transform.Find("/PressureReleaser/R_Grip").gameObject;
        leftHandAnim = transform.Find("/CameraPlaceholder/LeftHand").gameObject.GetComponent<HandAnimation>();
        rightHandAnim = transform.Find("/CameraPlaceholder/RightHand").gameObject.GetComponent<HandAnimation>();
        steam = transform.Find("/PressureReleaser/Steam/PressurisedSteam").gameObject.GetComponent<ParticleSystem>();
        gas = transform.Find("/PressureReleaser/Steam/PressurisedSteam/Gas").gameObject.GetComponent<ParticleSystem>();
        vibrate = transform.Find("/Vibration").GetComponent<VibrateController>();

        leftOriginalOrientation = leftGrip.transform.rotation;
        rightOriginalOrientation = rightGrip.transform.rotation;

        leftGrip.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
        rightGrip.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

        gas.Stop();
        steam.Stop();
    }
}
