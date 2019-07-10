using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TorqueLevel : MonoBehaviour
{
    public HandAnimation leftHand;
    public HandAnimation rightHand;
    public GameObject screwdriver;
    public Canvas canvas;
    private string stepListToggleButton1 = "joystick 1 button 5";
    private string stepListToggleButton2 = "joystick 2 button 5";
    private bool showingTorqueLevel = false;
    public string trackpadHorizontalLeftController;
    public string trackpadHorizontalRightController;
    private string taskRightController = "joystick 1 button 9";
    private string taskLeftController = "joystick 2 button 9";
    public float currentTorqueLevel;
    public float maximumTorqueLevel;
    public Text torqueText;
    // Start is called before the first frame update
    void Start()
    {
        canvas.gameObject.SetActive(false);
        currentTorqueLevel = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        float distLeftHandToScrew = Vector3.Distance(leftHand.transform.position, screwdriver.transform.position);
        float distRightHandToScrew = Vector3.Distance(rightHand.transform.position, screwdriver.transform.position);
        if ((leftHand.isGrabbing == true && distLeftHandToScrew <= 0.07f) || (rightHand.isGrabbing == true && distRightHandToScrew <= 0.07f)) {
            // Hand is grabbing the screwdriver
            if (Input.GetKeyDown(stepListToggleButton1) || Input.GetKeyDown(stepListToggleButton2)) {
                // Toggle the canvas on/off
                if (showingTorqueLevel == false) {
                    canvas.gameObject.SetActive(true);
                    showingTorqueLevel = true;
                } else if (showingTorqueLevel == true) {
                    canvas.gameObject.SetActive(false);
                    showingTorqueLevel = false;
                }
            }
        } 
        if (showingTorqueLevel == true) {
            if (((Input.GetAxis(trackpadHorizontalLeftController) > -1.0f && Input.GetAxis(trackpadHorizontalLeftController) <= 0f) && Input.GetKeyDown(taskLeftController))
                || ((Input.GetAxis(trackpadHorizontalRightController) >= -1.0f && Input.GetAxis(trackpadHorizontalRightController) <= 0f) && Input.GetKeyDown(taskRightController))) {
                // Click at the left side or up of the trackpad
                if (currentTorqueLevel > 0.5f) {
                    currentTorqueLevel -= 0.5f;
                }
            }
            if (((Input.GetAxis(trackpadHorizontalLeftController) > 0f && Input.GetAxis(trackpadHorizontalLeftController) <= 1f) && Input.GetKeyDown(taskLeftController))
                || ((Input.GetAxis(trackpadHorizontalRightController) >= 0f && Input.GetAxis(trackpadHorizontalRightController) <= 1f) && Input.GetKeyDown(taskRightController))) {
                // Click at the right side or down of the trackpad
                if (currentTorqueLevel < maximumTorqueLevel) {
                    currentTorqueLevel += 0.5f;
                }
            }
        } 
        if(leftHand.isGrabbing == false && rightHand.isGrabbing == false) {
            // Hand is not grabbing the screwdriver
            canvas.gameObject.SetActive(false);
            showingTorqueLevel = false;
        }
        
        torqueText.text = currentTorqueLevel + "";
    }
}
