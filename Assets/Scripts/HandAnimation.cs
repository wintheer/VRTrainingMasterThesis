using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimation : MonoBehaviour {

    private Animator _anim;
    private HandGrabbing _handGrab;
    public enum whichHand { leftHand, rightHand }
    public bool isGrabbing;

    public whichHand hand;
    public string rightControllerButton;
    public string leftControllerButton;

    // Use this for initialization
    void Start ()
    {
        _anim = GetComponentInChildren<Animator>();
        _handGrab = GetComponent<HandGrabbing>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        //if we are pressing grab, set animator bool IsGrabbing to true
        if (hand == whichHand.rightHand) {
            if (Input.GetKey(rightControllerButton)) {
                if (!_anim.GetBool("IsGrabbing")) {
                    isGrabbing = true;
                    _anim.SetBool("IsGrabbing", true);
                }
            } else {
                //if we let go of grab, set IsGrabbing to false
                if (_anim.GetBool("IsGrabbing")) {
                    isGrabbing = false;
                    _anim.SetBool("IsGrabbing", false);
                }
            }
        }

        if (hand == whichHand.leftHand) {
            if (Input.GetKey(leftControllerButton)) {
                if (!_anim.GetBool("IsGrabbing")) {
                    isGrabbing = true;
                    _anim.SetBool("IsGrabbing", true);
                }
            } else {
                //if we let go of grab, set IsGrabbing to false
                if (_anim.GetBool("IsGrabbing")) {
                    isGrabbing = false;
                    _anim.SetBool("IsGrabbing", false);
                }
            }
        }

    }
}