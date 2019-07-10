using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene1Plugs : MonoBehaviour
{
    //GameObjects
    public GameObject objectToAnimate;
    public GameObject target;

    //speed for movement and rotation
    public float moveSpeed = .005f;
    public float rotationSpeed = 18f;

    //time it takes to do one anitmation
    public float transformTime = 10;

    public Vector3 targetVector;

    private Vector3 initPos;
    private Quaternion initRot;

    private Vector3 targetPos;
    private Quaternion targetRot;

    private Vector3 aniPos;
    private Quaternion aniRot;

    void Start()
    {
        //initial position and rotation (of init)
        initPos = this.transform.position;
        initRot = this.transform.rotation;

        //target position and rotation
        targetPos = target.transform.position;
        targetRot = target.transform.rotation;

        //pos and rot for objectToAnimate
        aniPos = objectToAnimate.transform.position;
        aniRot = objectToAnimate.transform.rotation;

        moveSpeed = Vector3.Distance(this.transform.position, target.transform.position) / transformTime;
        rotationSpeed = Quaternion.Angle(this.transform.rotation, target.transform.rotation) / transformTime;
    }

    void Update()
    {
        if (Input.GetKey("a"))
        {
            Animate();
        }
    }

    private void Animate()
    {
        //Move from init to target
        objectToAnimate.transform.position = Vector3.MoveTowards(objectToAnimate.transform.position, targetPos, moveSpeed * Time.deltaTime);
        objectToAnimate.transform.rotation = Quaternion.RotateTowards(objectToAnimate.transform.rotation, targetRot, rotationSpeed * Time.deltaTime);

        Debug.Log("Object: " + objectToAnimate.transform.position);
        Debug.Log("Target: " + targetPos);

        if (objectToAnimate.transform.position == targetPos)
        {
            Debug.Log("Loop now");
            objectToAnimate.transform.position = initPos;
            objectToAnimate.transform.rotation = initRot;

        }
    }
}