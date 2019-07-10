using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;

public class Interaction_torque : MonoBehaviour {

    #region Frederik kode
    public Transform CurrentGrabObject
    {
        get { return _currentGrabObject; }
        set { _currentGrabObject = value; }
    }

    private Transform _currentGrabObject;
    #endregion
    private bool interacting = false;
    public GameObject pivot;

    public string trackpadLeft;
    public string trackpadRight;
    private string taskRightController = "joystick 1 button 9";
    private string taskLeftController = "joystick 2 button 9";
    public string grabbedScrew;

    public bool screwWasRemoved { get; set; }


    void Start() {
        XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
        _currentGrabObject = null;
    }

    void Update() {

        //Snapping screw to screwdriver
        if (_currentGrabObject == null) {

            //check for colliders in proximity
            Collider[] colliders = Physics.OverlapSphere(pivot.transform.position, 0);
            List<Collider> colsWithTag = new List<Collider>();
            if (colliders.Length > 0) {
                
                foreach (Collider col in colliders) {
                    if (col.CompareTag("Screw")) {
                        colsWithTag.Add(col);
                    }
                }

                if (colsWithTag.Count > 0) {
                    //We have at least one screw now we need to pick the closest one of all if there is more than one
                    Collider closestScrew = colsWithTag[0];
                    foreach (Collider screw in colsWithTag) {
                        if (Vector3.Distance(pivot.transform.position, screw.transform.position) 
                            < Vector3.Distance(closestScrew.transform.position, screw.transform.position)) {
                            closestScrew = screw;
                        }
                    }
                    grabbedScrew = closestScrew.name;

                    //The screw will now be set as a child to the screwdriver
                    closestScrew.transform.SetParent(transform);

                    _currentGrabObject = closestScrew.transform;

                }
            }
        }

        if (screwWasRemoved) {
            screwWasRemoved = false;
            _currentGrabObject = null;
        }


        // The screw is on the screwdriver. Lets see if it is inside a snappingbox!

            interacting = false;
        //float dist = Vector3.Distance(screw.transform.position, pivot.transform.position);
        if ((Input.GetAxis(trackpadLeft) == 1.0f && Input.GetKeyDown(taskLeftController)) || (Input.GetAxis(trackpadRight) == 1.0f && Input.GetKeyDown(taskRightController))) {
            // Click at the bottom part of the trackpad
            
        }
        if (Input.GetAxis(trackpadLeft) == -1.0f && Input.GetKeyDown(taskLeftController) || (Input.GetAxis(trackpadRight) == -1.0f && Input.GetKeyDown(taskRightController))) {
            // Click at the top part of the trackpad
            
        }
        
        
    }
}

