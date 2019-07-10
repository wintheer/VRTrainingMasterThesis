using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComplexScrewAnimation : MonoBehaviour
{
    public GameObject middleObjectPosition;
    public GameObject objectToAnimate;
    public GameObject washer;
    public bool done = false;
    private Vector3 initPos;
    private Vector3 middlePos;
    private Vector3 targetPos;
    public float speed = 0.5f;
    private  int stepNumber = 1;
    // Start is called before the first frame update
    void Start()
    {
        initPos = this.transform.position;
        middlePos = middleObjectPosition.transform.position;
        targetPos = objectToAnimate.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Animate()
    {
        
        if(stepNumber == 1){
        //move from A to B
        float step = speed * Time.deltaTime; // calculate distance to move
        objectToAnimate.transform.position = Vector3.MoveTowards(objectToAnimate.transform.position, middleObjectPosition.transform.position, step);
        objectToAnimate.transform.rotation = Quaternion.RotateTowards(objectToAnimate.transform.rotation, middleObjectPosition.transform.rotation, step*100);
        washer.transform.position = Vector3.MoveTowards(washer.transform.position, middleObjectPosition.transform.position, step);
        washer.transform.rotation = Quaternion.RotateTowards(washer.transform.rotation, middleObjectPosition.transform.rotation, step*100);
        if(Vector3.Distance(objectToAnimate.transform.position, middleObjectPosition.transform.position) < 0.001f && 
        Vector3.Distance(washer.transform.position, middleObjectPosition.transform.position) < 0.001f){
            stepNumber += 1;
        }
        }
        else if(stepNumber == 2){
        float step = speed * Time.deltaTime; // calculate distance to move
        objectToAnimate.transform.position = Vector3.MoveTowards(objectToAnimate.transform.position, this.transform.position, step);
        objectToAnimate.transform.rotation = Quaternion.RotateTowards(objectToAnimate.transform.rotation, this.transform.rotation, step*100);
        washer.transform.position = Vector3.MoveTowards(washer.transform.position, this.transform.position, step);
        washer.transform.rotation = Quaternion.RotateTowards(washer.transform.rotation, this.transform.rotation, step*100);
        //I am done animating - tell the others
        if (Vector3.Distance(objectToAnimate.transform.position, this.transform.position) < 0.001f && 
        Vector3.Distance(washer.transform.position, this.transform.position) < 0.001f)
        {
            Debug.Log("I am done animating");
            //repeat animation
            //objectToAnimate.transform.position = targetPos;

            // add gravity and shit
            Mesh mesh = objectToAnimate.GetComponentInChildren<MeshFilter>().mesh;
            Mesh mesh2 = washer.GetComponentInChildren<MeshFilter>().mesh;
            BoxCollider boxCollider = objectToAnimate.AddComponent<BoxCollider>();
            BoxCollider boxCollider2 = washer.AddComponent<BoxCollider>();
            boxCollider.size = mesh.bounds.size;
            boxCollider2.size = mesh.bounds.size;
            boxCollider.center = mesh.bounds.center;
            boxCollider2.center = mesh.bounds.center;
            objectToAnimate.AddComponent<Rigidbody>();
            washer.AddComponent<Rigidbody>();
            objectToAnimate.GetComponent<Rigidbody>().useGravity = true;
            washer.GetComponent<Rigidbody>().useGravity = true;
            objectToAnimate.GetComponent<Rigidbody>().AddTorque(1, 0, 0);
            washer.GetComponent<Rigidbody>().AddTorque(1, 0, 0);
            done = true;
        }
        }
    }
}
