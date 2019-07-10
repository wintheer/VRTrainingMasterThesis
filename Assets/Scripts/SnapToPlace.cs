using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * This class takes two positions where the object will snap to. 
 * Whenever it is near one of these two objects,
 * it will snap to them if it is not grabbed.
 */
public class SnapToPlace : MonoBehaviour
{
    // Enum is used to determine which place should be highlighted.
    public enum State { Assembly, Disassembly }
    public GameObject SnappingPositionsPrefab;
    private GameObject OriginalSnappingObjPos, DisassembledSnappingObjPos;
    public int ObjectID;
    public State WhichState = State.Disassembly;
    public bool IsGrabbed = false;
    public bool ownShouldActivate;
    public bool TakenByScrewdriver = false;

    private readonly string ParentRight = "RightHand";
    private readonly string ParentLeft = "LeftHand";
    public bool HasBeenSnapped { get; set; }
    public bool ShouldActivate { get; set; }


    public void Start() {
        OriginalSnappingObjPos = SnappingPositionsPrefab.transform.GetChild(0).gameObject;
        DisassembledSnappingObjPos = SnappingPositionsPrefab.transform.GetChild(1).gameObject;

        DisassembledSnappingObjPos.SetActive(false);
        OriginalSnappingObjPos.SetActive(false);
    }

    // Update is called once per frame
    void Update() {
        HasHandParent();
        IsInsideSnappingPoint();
        ownShouldActivate = ShouldActivate;
    }

    private void HasHandParent() {

        if (transform.parent != null) {

            if (transform.parent.name.Equals(ParentRight) || transform.parent.name.Equals(ParentLeft)) {
                IsGrabbed = true;
            } else {
                IsGrabbed = false;
            }

            //is it grabbed by a screwdriver?
            if (transform.parent.name.Equals("Torque") || transform.parent.name.Equals("Needle_tool")) {
                TakenByScrewdriver = true;
            }
        } else {
            IsGrabbed = false;
        }
    }

    private void IsInsideSnappingPoint() {
        // If the object is released then it should check if the object is valid to snap to one of the two positions
        if (!IsGrabbed && ShouldActivate && !TakenByScrewdriver) {
            if (WhichState == State.Assembly) {
                // Is it inside the original position?
                if (OriginalSnappingObjPos.GetComponent<ObjectIsInsideSnapping>().IsTriggered) {

                    if (!HasBeenSnapped) {
                        OriginalSnappingObjPos.SetActive(false);
                        HasBeenSnapped = true;
                        transform.position = OriginalSnappingObjPos.transform.position;
                        transform.rotation = OriginalSnappingObjPos.transform.rotation;
                        transform.GetComponent<Rigidbody>().useGravity = false;
                    }
                } else {
                    HasBeenSnapped = false;
                }
                if (HasBeenSnapped) {
                    //GetComponent<Rigidbody>().isKinematic = false;
                }
            }
            else if (WhichState == State.Disassembly) {
                // Is it inside the desired destination when it is disassembled?
                if (DisassembledSnappingObjPos.GetComponent<ObjectIsInsideSnapping>().IsTriggered) {

                    if (!HasBeenSnapped) {
                        DisassembledSnappingObjPos.SetActive(false);
                        HasBeenSnapped = true;
                        transform.position = DisassembledSnappingObjPos.transform.position;
                    } 
                } else {
                    HasBeenSnapped = false;
                }
            }
        }
        if (TakenByScrewdriver && ShouldActivate) { // If it's a screw it needs to be able to unsnap from screwdriver
            
            if (WhichState == State.Assembly) {
                // Are we dealing with a screw?
                if (OriginalSnappingObjPos.GetComponent<ObjectIsInsideSnapping>().IsTriggered) {
                    if (!HasBeenSnapped) {
                        OriginalSnappingObjPos.SetActive(false);
                        // Set the screws parent to be the pump
                        Transform parentObject = GameObject.Find("DetailedPump2").transform;
                        HasBeenSnapped = true;

                        // Tell the screwdriver we're not a child anymore
                        transform.parent.GetComponent<Interaction_torque>().screwWasRemoved = true;

                        transform.SetParent(parentObject);
                        transform.position = OriginalSnappingObjPos.transform.position;
                        transform.rotation = OriginalSnappingObjPos.transform.rotation;
                        transform.GetComponent<Rigidbody>().useGravity = false;
                    }
                } else {
                    HasBeenSnapped = false;
                }
            }
            else if (WhichState == State.Disassembly) {
                // Are we dealing with a screw?
                if (DisassembledSnappingObjPos.GetComponent<ObjectIsInsideSnapping>().IsTriggered) {
                    
                    if (!HasBeenSnapped) {
                        DisassembledSnappingObjPos.SetActive(false);

                        // Tell the screwdriver we're not a child anymore
                        transform.parent.GetComponent<Interaction_torque>().screwWasRemoved = true;

                        // The screw should not have a parent then!
                        transform.SetParent(null);
                        HasBeenSnapped = true;
                        transform.position = DisassembledSnappingObjPos.transform.position;
                    }
                } else {
                    HasBeenSnapped = false;
                }
            }
        }
        if (IsGrabbed && !HasBeenSnapped || TakenByScrewdriver && !HasBeenSnapped) {
            // Object is grabbed meaning it has not been snapped to a position!
            HasBeenSnapped = false;
            DisplayRightSnappingHint();
        }
    }

    public int GetObjectID() {
        return this.ObjectID;
    }

    private void DisplayRightSnappingHint() {

        if (ShouldActivate) {
            if (WhichState == State.Assembly) {
                OriginalSnappingObjPos.SetActive(true);
                DisassembledSnappingObjPos.SetActive(false);
            } else {
                OriginalSnappingObjPos.SetActive(false);
                DisassembledSnappingObjPos.SetActive(true);
            }
        }
    }

    public void ResetSnapping() {
        HasBeenSnapped = false;
    }
}
