using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrewTest1 : MonoBehaviour
{

    //this script should be generic to all objects thats needs to be animated

    public float transformTime = .5f;
    public GameObject target;
    public int numberOfMoves;
    private float moveSpeed;
    private float rotationSpeed;
    private bool animationDone = false;
    private bool animating = false;

    //initial position and rotation
    private Vector3 initPos;
    private Quaternion initRot;

    //changes to object color
    private float duration = 0.5f;

    //name of target
    private string targetName;

    void Start()
    {
        targetName = this.name;
        var replacement  = targetName.Replace("Init", "Target");
        target = GameObject.Find(replacement);
        //defines move speed and initial rotation and position of object (this)
        moveSpeed = Vector3.Distance(this.transform.position, target.transform.position) / transformTime;
        rotationSpeed = Quaternion.Angle(this.transform.rotation, target.transform.rotation) / transformTime;
        initPos = this.transform.position;
        initRot = this.transform.rotation;

        //makes the animation running
        animating = true;
    }

    void Update()
    {
        //condition for starting animation
        if (animating && !animationDone)
        {
            Animate();
            //Debug.Log("Unscrewing...");
        }
    }

    private void Animate()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        this.transform.rotation = Quaternion.RotateTowards(this.transform.rotation, target.transform.rotation, rotationSpeed * Time.deltaTime);

        if (this.transform.position == target.transform.position)
        {
            animationDone = true;
            //Debug.Log("unscrewed!!!");
            this.transform.position = initPos;
            this.transform.rotation = initRot;
            animationDone = false;
        }
    }
}