using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxBehavior : MonoBehaviour {

    private GameObject lScrew1, lScrew2, lScrew3, lScrew4;
    private GameObject tScrew1, tScrew2, tScrew3, tScrew4;
    private GameObject valve;
    private GameObject leftThumb, rightThumb, leftButton, rightButton;

    public GameObject rocket;
    public ParticleSystem flames;
    public float force = 1;

    private Vector3 initialPosRocket;

    public System.Diagnostics.Stopwatch stopwatch;

    public bool screw1Loosened, screw2Loosened, screw3Loosened, screw4Loosened;
    private bool screw1Tightened, screw2Tightened, screw3Tightened, screw4Tightened;
    public bool playedAnimation = false;
    private bool buttonsPressed = false;

    private readonly string screwTag = "Screw";
    private readonly string grabTag = "Grab";
    private readonly string unTag = "Untagged";
    private GameObject sound;

    void Start() {
        FindComponents();
        sound.GetComponent<PlaySound>().Speak(1);
    }

    void Update() {
        ScrewLoosen();
        ScrewTighten();
    }

    private void FixedUpdate() {
        LaunchRocket();
    }

    private void FindComponents() {
        lScrew1 = GameObject.Find("/LScrews/LScrew1");
        lScrew2 = GameObject.Find("/LScrews/LScrew2");
        lScrew3 = GameObject.Find("/LScrews/LScrew3");
        lScrew4 = GameObject.Find("/LScrews/LScrew4");

        tScrew1 = GameObject.Find("/TScrews/TScrew1");
        tScrew2 = GameObject.Find("/TScrews/TScrew2");
        tScrew3 = GameObject.Find("/TScrews/TScrew3");
        tScrew4 = GameObject.Find("/TScrews/TScrew4");

        valve = GameObject.Find("/Valve/Valve");
        valve.GetComponent<HandLoosen>().shouldActivate = true;

        leftThumb = GameObject.Find("/CameraPlaceholder/LeftHand/LeftHandSkinnedMesh/hands:l_hand_world/hands:b_l_hand" +
            "/hands:b_l_thumb1/hands:b_l_thumb2/hands:b_l_thumb3/hands:b_l_thumb_ignore");
        rightThumb = GameObject.Find("/CameraPlaceholder/RightHand/RightHandSkinnedMesh/hands:l_hand_world/hands:b_l_hand" +
            "/hands:b_l_thumb1/hands:b_l_thumb2/hands:b_l_thumb3/hands:b_l_thumb_ignore");
        leftButton = GameObject.Find("/Rocket/ButtonLeft");
        rightButton = GameObject.Find("/Rocket/ButtonRight");

        leftButton.GetComponent<GlowObjectCmd>().shouldGlow = true;
        rightButton.GetComponent<GlowObjectCmd>().shouldGlow = true;

        flames.Stop();
        initialPosRocket = rocket.transform.position;

        sound = GameObject.Find("Speech");
    }

    private void ScrewLoosen() {

        if (!screw1Loosened) {
            screw1Loosened = ActivateDeactivateScrew(lScrew1, SnapToPlace.State.Disassembly);
        } else if (!screw2Loosened) {
            screw2Loosened = ActivateDeactivateScrew(lScrew2, SnapToPlace.State.Disassembly);
        } else if (!screw3Loosened) {
            screw3Loosened = ActivateDeactivateScrew(lScrew3, SnapToPlace.State.Disassembly);
        } else if (!screw4Loosened) {
            screw4Loosened = ActivateDeactivateScrew(lScrew4, SnapToPlace.State.Disassembly);
        }
    }

    private void ScrewTighten() {

        if (!screw1Tightened) {
            screw1Tightened = ActivateDeactivateScrew(tScrew1, SnapToPlace.State.Assembly);
        } else if (!screw2Tightened) {
            screw2Tightened = ActivateDeactivateScrew(tScrew2, SnapToPlace.State.Assembly);
        } else if (!screw3Tightened) {
            screw3Tightened = ActivateDeactivateScrew(tScrew3, SnapToPlace.State.Assembly);
        } else if (!screw4Tightened) {
            screw4Tightened = ActivateDeactivateScrew(tScrew4, SnapToPlace.State.Assembly);
        }
    }

    private bool ActivateDeactivateScrew(GameObject screw, SnapToPlace.State state) {
        //screw.GetComponent<TightenAction>().shouldActivate = true;
        screw.GetComponent<SnapToPlace>().ShouldActivate = true;
        screw.GetComponent<GlowObjectCmd>().shouldGlow = true;
        screw.GetComponent<SnapToPlace>().WhichState = state;

        if (state == SnapToPlace.State.Disassembly) {
            screw.GetComponent<TightenAction>().tightenOrLoosen = TightenAction.TightenOrLoosen.Loosen;
            screw.GetComponent<TightenAction>().shouldActivate = true;
            if (screw.GetComponent<TightenAction>().doneTightening) {
                screw.tag = screwTag;
                screw.GetComponent<TightenAction>().shouldActivate = false;
            }

            if (screw.GetComponent<SnapToPlace>().HasBeenSnapped) {
                screw.tag = unTag;
                screw.GetComponent<TightenAction>().shouldActivate = false;
                screw.GetComponent<GlowObjectCmd>().shouldGlow = false;
                screw.GetComponent<SnapToPlace>().ShouldActivate = false;
                //screw.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                return true;
            }
            return false;

        } else {
            screw.tag = grabTag;
            screw.GetComponent<TightenAction>().tightenOrLoosen = TightenAction.TightenOrLoosen.Tighten;

            if (screw.GetComponent<SnapToPlace>().HasBeenSnapped) {
                screw.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                screw.tag = unTag;
                screw.GetComponent<TightenAction>().shouldActivate = true;
                screw.GetComponent<GlowObjectCmd>().glowSpecificColor(new Color(0.850f, 1.0f, 0f, 1.0f));
                if (screw.GetComponent<TightenAction>().doneTightening) {
                    screw.GetComponent<GlowObjectCmd>().shouldGlow = false;
                    screw.GetComponent<TightenAction>().shouldActivate = false;
                    return true;
                }
            }
        }
        return false;
    }

    private bool timerHasBeenReset;

    private void LaunchRocket() {
        // Get distance between left button and left hand
        float distanceLeft = Vector3.Distance(leftThumb.transform.position, leftButton.transform.position);
        // Get distance between right button and right hand
        float distanceRight = Vector3.Distance(rightThumb.transform.position, rightButton.transform.position);

        if (stopwatch == null) {
            stopwatch = new System.Diagnostics.Stopwatch();
        }

        if (stopwatch.ElapsedMilliseconds > 6800) {
            ResetRocket();
            timerHasBeenReset = false;
            buttonsPressed = false;
        }


        if (distanceLeft < 0.2f && distanceRight < 0.2f) {
            if (!stopwatch.IsRunning) {
                stopwatch.Start();
            }
            if (stopwatch.ElapsedMilliseconds >= 1800 && stopwatch.ElapsedMilliseconds < 2000) {
                buttonsPressed = true;
            } 
        } 
        else {
            if (!timerHasBeenReset) {
                stopwatch.Reset();
                timerHasBeenReset = true;
            }
            
        }


        if (buttonsPressed) {
            // LAUNCH ROCKET AND PLAY ANIMATION!!!!!
            rocket.GetComponent<Rigidbody>().AddForce(force * rocket.transform.up * Time.deltaTime, ForceMode.Acceleration);
            if (!playedAnimation) {
                playedAnimation = true;
                flames.Play();
            }
        }
    }

    private void ResetRocket() {
        playedAnimation = false;
        stopwatch.Stop();
        stopwatch = new System.Diagnostics.Stopwatch();
        rocket.transform.position = initialPosRocket;
        flames.Stop();
        buttonsPressed = false;
        rocket.GetComponent<Rigidbody>().velocity = Vector3.zero;
        rocket.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
    }
}
