using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene2Steps : MonoBehaviour
{
    private GameObject backLid;

    public void LidOff()

    {
        backLid = GameObject.Find("Back_lid");
        backLid.GetComponent<Rigidbody>().useGravity = true;
        backLid.GetComponent<Rigidbody>().AddTorque(1, 0, 0);
    }
}
