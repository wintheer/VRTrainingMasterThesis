using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PunctureWater : MonoBehaviour {

    public GameObject WaterPrebab;
    private AudioSource audioData;
    void Start() {
        audioData = GetComponent<AudioSource>();
    }

    void Update() {
        
    }

    private void OnCollisionEnter(Collision collision) {

        if (collision.transform.name.Equals("Pivot")) {
            Vector3 point = collision.contacts[0].point;
            Vector3 pivotRotation = collision.transform.rotation.eulerAngles;

            Vector3 reverseRotation = new Vector3(pivotRotation.x, pivotRotation.y + 90, pivotRotation.z);
            //print("Points colliding: " + collision.contacts[0].point);
            Instantiate(WaterPrebab, point, Quaternion.Euler(reverseRotation));

            if (!audioData.isPlaying) {
                audioData.Play();
            }
        }
    }

    private void OnCollisionExit(Collision collision) {
        
    }
}
