using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR; //needs to be UnityEngine.VR in Versions before 2017.2

public class HandGrabbing : MonoBehaviour {

    public string InputName;
    public HandGrabbing OtherHandReference;
    public XRNode NodeType;
    public Vector3 ObjectGrabOffset;
    public float GrabDistance = 0.1f;
    public string GrabTag = "Grab";
    public float ThrowMultiplier = 1.5f;
    public string controllerButton;
    public float yRotation, zRotation, xPositionOffset, yPositionOffset, zPositionOffset;
    public float xRotation = 0.15f;
    public GameObject toolRef;

    public Transform CurrentGrabObject {
        get { return _currentGrabObject; }
        set { _currentGrabObject = value; }
    }

    private Vector3 _lastFramePosition;
    private Transform _currentGrabObject;
    private bool _isGrabbing;
    private bool hasParent;
    private Transform parentObject;

    // Use this for initialization
    void Start() {
        _lastFramePosition = transform.position;
        XRDevice.SetTrackingSpaceType(TrackingSpaceType.RoomScale);
        _currentGrabObject = null;
        _isGrabbing = false;
    }

    // Update is called once per frame
    void Update() {
        Vector3 handPosition = InputTracking.GetLocalPosition(NodeType);
        Quaternion handRotation = InputTracking.GetLocalRotation(NodeType);

        //update hand position and rotation
        transform.localPosition = handPosition;
        transform.localRotation = handRotation;

        //if we don't have an active object in hand, look if there is one in proximity
        if (_currentGrabObject == null) {
            //check for colliders in proximity
            Collider[] closestObjects = Physics.OverlapSphere(transform.GetChild(1).transform.position, GrabDistance);
            List<Collider> colsWithTag = new List<Collider>();
            if (closestObjects.Length > 0) {
                foreach (Collider col in closestObjects) {
                    if (col.CompareTag("Grab")) {
                        colsWithTag.Add(col);
                    }
                }
                if (colsWithTag.Count > 0) {
                    //We have at least one object now we need to pick the closest one of all if there is more than one
                    Collider closestObject = colsWithTag[0];
                    foreach (Collider grabObject in colsWithTag) {
                        if (Vector3.Distance(transform.GetChild(1).transform.position, grabObject.transform.position) 
                            < Vector3.Distance(closestObject.transform.position, grabObject.transform.position)) {
                            closestObject = grabObject;
                        }
                    }

                    //if there are colliders, take the first one if we press the grab button and it has the tag for grabbing
                    if (Input.GetKey(controllerButton) && closestObject.transform.CompareTag(GrabTag)) {
                        //if we are already grabbing, return
                        if (_isGrabbing) {
                            return;
                        }
                        _isGrabbing = true;

                        // this is where the snapping needs to happen FREDDY
                        closestObject.transform.rotation = InputTracking.GetLocalRotation(NodeType);

                        // Get the current parent for later.
                        if (closestObject.transform.parent != null) {
                            hasParent = true;
                            parentObject = closestObject.transform.parent;
                        }


                        //set current object to the object we have picked up (set it as child)
                        closestObject.transform.SetParent(transform);
                        closestObject.transform.position = toolRef.transform.position;
                        closestObject.transform.rotation = toolRef.transform.rotation;
                        // = Quaternion.Euler(-this.gameObject.transform.GetChild(0).localRotation.x + xRotation, this.gameObject.transform.GetChild(0).localRotation.y+yRotation, this.gameObject.transform.GetChild(0).localRotation.z + zRotation);
                        // Set grabbed object's position inside hand position

                        //if there is no rigidbody to the grabbed object attached, add one
                        if (closestObject.GetComponent<Rigidbody>() == null) {
                            closestObject.gameObject.AddComponent<Rigidbody>();
                        }

                        closestObject.GetComponent<Rigidbody>().isKinematic = true;

                        //save a reference to grab object
                        _currentGrabObject = closestObject.transform;

                        //does other hand currently grab object? then release it!
                        if (OtherHandReference.CurrentGrabObject != null) {
                            OtherHandReference.CurrentGrabObject = null;
                        }
                    }
                }
            }
        } else { //we have object in hand, update its position with the current hand position

            //if we we release grab button, release current object
            if (!Input.GetKey(controllerButton)) {

                //set grab object to non-kinematic (enable physics)
                Rigidbody _objectRGB = _currentGrabObject.GetComponent<Rigidbody>();
                _objectRGB.isKinematic = false;
                _objectRGB.velocity = Vector3.zero;
                _objectRGB.collisionDetectionMode = CollisionDetectionMode.Continuous;

                //release the the object (unparent it)
                _currentGrabObject.SetParent(null);

                //release reference to object
                _currentGrabObject = null;
            }
        }

        //release grab ?
        if (!Input.GetKey(controllerButton) && _isGrabbing) {
            _isGrabbing = false;
        }
        //save the current position for calculation of velocity in next frame
        _lastFramePosition = transform.position;
    }
}