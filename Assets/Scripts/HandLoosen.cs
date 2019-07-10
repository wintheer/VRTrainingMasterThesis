using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandLoosen : MonoBehaviour {

    private HandAnimation LeftHand, RightHand;
    private bool shouldCalculateDistance = true;
    private float initialDistance, distanceToRightHand, distanceToLeftHand;
    private float interactionDistance = 0.1f;

    public enum WhichAxis { xAxis, yAxis, zAxis }
    public enum ActionType { Tighten, Loosen }
    public enum MoveUpOrDown { Nothing, Up, Down }
    private float previousRotation;
    private float totalRotated;

    public float rotatingDegrees = 360;
    public WhichAxis AxisToMoveOn;
    public ActionType TightenOrLoosen;
    public MoveUpOrDown moveUpOrDown;

    private enum HandType { Left, Right }

    public bool doneRotating { get; set; }
    public bool shouldActivate { get; set; }

    public GameObject destinationEmpty;
    void Start() {
        LeftHand = GameObject.Find("/CameraPlaceholder/LeftHand").GetComponent<HandAnimation>();
        RightHand = GameObject.Find("/CameraPlaceholder/RightHand").GetComponent<HandAnimation>();
    }

    void Update() {
        if (shouldActivate) {
            distanceToRightHand = Vector3.Distance(transform.position, RightHand.transform.position);
            distanceToLeftHand = Vector3.Distance(transform.position, LeftHand.transform.position);

            if (shouldCalculateDistance) {
                shouldCalculateDistance = false;
                initialDistance = Vector3.Distance(transform.position, destinationEmpty.transform.position);
            }

            if (distanceToLeftHand < distanceToRightHand) {
                // Use functionality for left hand
                rotateSelf(HandType.Left);

            } else {
                // Use functionality for right hand
                rotateSelf(HandType.Right);
            }
        }
    }

    void rotateSelf(HandType hand) {

        #region Right Hand
        if (hand == HandType.Right) {
            if (RightHand.isGrabbing) {
                if (distanceToRightHand <= interactionDistance) {
                    float rotatedAmount = RightHand.transform.localEulerAngles.z - previousRotation;
                    if (rotatedAmount > 0 && TightenOrLoosen == ActionType.Loosen || rotatedAmount < 0 && TightenOrLoosen == ActionType.Tighten) {

                        if (Mathf.Abs(rotatedAmount) > 100) {
                            rotatedAmount = 0;
                        }

                        float rotatingFraction = Mathf.Abs(rotatedAmount / rotatingDegrees);

                        totalRotated += rotatedAmount;

                        Vector3 movement;
                        if (AxisToMoveOn == WhichAxis.xAxis) {
                            movement = new Vector3(rotatingFraction * initialDistance, 0, 0);
                            transform.Rotate(rotatedAmount, 0, 0, Space.Self);
                        } else if (AxisToMoveOn == WhichAxis.yAxis) {
                            movement = new Vector3(0, rotatingFraction * initialDistance, 0);
                            transform.Rotate(0, rotatedAmount, 0, Space.Self);
                        } else {
                            movement = new Vector3(0, 0, rotatingFraction * initialDistance);
                            transform.Rotate(0, 0, rotatedAmount, Space.Self);
                        }

                        if (Vector3.Distance(transform.position, destinationEmpty.transform.position) <= 0.0005) {
                            doneRotating = true;
                        }

                        if (!doneRotating) {
                            transform.position = Vector3.MoveTowards(transform.position, destinationEmpty.transform.position, rotatingFraction * initialDistance);
                        }
                    }
                }
                previousRotation = RightHand.transform.localEulerAngles.z;
            } else {
                previousRotation = 0;
            }
        }
        #endregion

        #region LeftHand
        if (hand == HandType.Left) {
            if (LeftHand.isGrabbing) {
                if (distanceToLeftHand <= interactionDistance) {
                    float rotatedAmount = LeftHand.transform.localEulerAngles.z - previousRotation;
                    if (rotatedAmount > 0 && TightenOrLoosen == ActionType.Loosen || rotatedAmount < 0 && TightenOrLoosen == ActionType.Tighten) {

                        if (Mathf.Abs(rotatedAmount) > 100) {
                            rotatedAmount = 0;
                        }

                        float rotatingFraction = Mathf.Abs(rotatedAmount / rotatingDegrees);

                        totalRotated += rotatedAmount;

                        Vector3 movement;
                        if (AxisToMoveOn == WhichAxis.xAxis) {
                            movement = new Vector3(rotatingFraction * initialDistance, 0, 0);
                            transform.Rotate(rotatedAmount, 0, 0, Space.Self);
                        } else if (AxisToMoveOn == WhichAxis.yAxis) {
                            movement = new Vector3(0, rotatingFraction * initialDistance, 0);
                            transform.Rotate(0, rotatedAmount, 0, Space.Self);
                        } else {
                            movement = new Vector3(0, 0, rotatingFraction * initialDistance);
                            transform.Rotate(0, 0, rotatedAmount, Space.Self);
                        }

                        if (Vector3.Distance(transform.position, destinationEmpty.transform.position) <= 0.0005) {
                            doneRotating = true;
                        }

                        if (!doneRotating) {
                            transform.position = Vector3.MoveTowards(transform.position, destinationEmpty.transform.position, rotatingFraction * initialDistance);
                        }
                    }
                }
                previousRotation = LeftHand.transform.localEulerAngles.z;
            } else {
                previousRotation = 0;
            }
        }
        #endregion
    }

    public float TotalRotation() {
        float toReturn = Mathf.Abs(totalRotated / rotatingDegrees);
        if (toReturn > 1f)
            toReturn = 1;
        return toReturn;
    }

    public void resetRotation() {
        totalRotated = 0;
    }
}
