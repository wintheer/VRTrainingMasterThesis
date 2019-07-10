using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleAnimation : MonoBehaviour
{
    public GameObject objectToAnimate;
    public bool done = false;
    private Vector3 initPos;
    private Vector3 targetPos;

    private Vector3 deltaPos;

    public float speed = 10;

    void Start()
    {
        initPos = this.transform.position;
        targetPos = objectToAnimate.transform.position;




    }

    void Update()
    {
        if (Input.GetKey("a"))
        {
            Animate();
        }

    }

    public void Animate()
    {
        //move from A to B
        float step = speed * Time.deltaTime; // calculate distance to move
        objectToAnimate.transform.position = Vector3.MoveTowards(objectToAnimate.transform.position, this.transform.position, step);
        objectToAnimate.transform.rotation = Quaternion.RotateTowards(objectToAnimate.transform.rotation, this.transform.rotation, step*100);

        //I am done animating - tell the others
        if (Vector3.Distance(objectToAnimate.transform.position, this.transform.position) < 0.001f)
        {
            Debug.Log("I am done animating");
            //repeat animation
            //objectToAnimate.transform.position = targetPos;

            // add gravity and shit
            Mesh mesh = objectToAnimate.GetComponentInChildren<MeshFilter>().mesh;
            BoxCollider boxCollider = objectToAnimate.AddComponent<BoxCollider>();
            boxCollider.size = mesh.bounds.size;
            boxCollider.center = mesh.bounds.center;
            objectToAnimate.AddComponent<Rigidbody>();
            objectToAnimate.GetComponent<Rigidbody>().useGravity = true;
            objectToAnimate.GetComponent<Rigidbody>().AddTorque(1, 0, 0);
            done = true;
        }
    }
}
