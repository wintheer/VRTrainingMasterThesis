using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandboxScript : MonoBehaviour {

    public ParticleSystem steam, gas;
    public GameObject handle;
    public HandLoosen loosen;
    private GameObject LeftHand, RightHand, Hint, screw1;
    private AudioSource audioData;

    private Vector3 originalValvePos;

    private bool playedAnimation;
    void Start() {
        steam.Stop();
        gas.Stop();
        loosen = handle.GetComponent<HandLoosen>();

        LeftHand = GameObject.Find("/CameraPlaceholder/LeftHand");
        RightHand = GameObject.Find("/CameraPlaceholder/RightHand");
        Hint = GameObject.Find("/Hint1");
        screw1 = GameObject.Find("BehaviorScript");
        audioData = GetComponent<AudioSource>();

        originalValvePos = GameObject.Find("/Valve/Valve").transform.position;
    }

    void Update() {
        if (loosen.doneRotating && !playedAnimation) {
            playedAnimation = true;
            gas.Play();
            steam.Play();
            if (!audioData.isPlaying) {
                audioData.Play();
            }

            ResetValve();
        }
        if (screw1.GetComponent<SandboxBehavior>().screw1Loosened == true) {
            Hint.SetActive(false);
        }

        float distanceToLeftHand = -10;
        float distanceToRightHand = -10;
        distanceToLeftHand = Vector3.Distance(LeftHand.transform.position, Hint.transform.position);
        distanceToLeftHand = Vector3.Distance(RightHand.transform.position, Hint.transform.position);

        if ((distanceToRightHand > -10 || distanceToLeftHand > -10) && screw1.GetComponent<SandboxBehavior>().screw1Loosened == false) {
            if ((distanceToLeftHand < 0.1f && distanceToLeftHand >= 0) || (distanceToRightHand < 0.1f && distanceToRightHand > 0)) {
                Hint.SetActive(false);
            } else {
                Hint.SetActive(true);
            }
        }
    }

    private void ResetValve() {
        GameObject.Find("/Valve/Valve").transform.position = originalValvePos;
        loosen.doneRotating = false;
        playedAnimation = false;
    }
}
