using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloves : MonoBehaviour
{
    private GameObject humanLefthand, humanRighthand, leftGlove, rightGlove;
    private GameObject camera;
    private GameObject safetyGoggles;
    public string rightControllerButton, leftControllerButton;
    public bool safetyOn, leftGloveDone, rightGloveDone, safetyGogglesDone;
    private float glovesTrigger = 0.10f;
    // Start is called before the first frame update
    void Start()
    {
        FindRelevantObjects();
    }

    // Update is called once per frame
    void Update()
    {
        WearGlove();
        WearGoggles();
    }
    private void FindRelevantObjects()
    {
        humanLefthand = GameObject.Find("LeftHumanHand");
        humanRighthand = GameObject.Find("RightHumanHand");
        leftGlove = GameObject.Find("LeftGlove");
        rightGlove = GameObject.Find("RightGlove");
        camera = GameObject.Find("Main Camera");
        safetyGoggles = GameObject.Find("SafetyGogglesPlaceholder");
    }
    private void WearGlove()
    {
        if (leftGlove.transform.parent != transform && humanLefthand.gameObject != null)
        {
            float distLeftGloveToLeftHumanHand = Vector3.Distance(leftGlove.transform.position, humanLefthand.transform.position);
            if (distLeftGloveToLeftHumanHand <= glovesTrigger)
            {
                leftGlove.transform.SetParent(transform);
                leftGlove.transform.GetComponent<HandGrabbing>().enabled = true;
                leftGlove.transform.GetComponent<HandAnimation>().enabled = true;
                leftGloveDone = true;
                Destroy(humanLefthand.gameObject);
            }
        }
        if (rightGlove.transform.parent != transform && humanRighthand.gameObject != null)
        {
            float distRightGloveToRightHumanHand = Vector3.Distance(rightGlove.transform.position, humanRighthand.transform.position);
            if (distRightGloveToRightHumanHand <= glovesTrigger)
            {
                rightGlove.transform.SetParent(transform);
                rightGlove.transform.GetComponent<HandGrabbing>().enabled = true;
                rightGlove.transform.GetComponent<HandAnimation>().enabled = true;
                rightGloveDone = true;
                Destroy(humanRighthand.gameObject);
            }
        }
        if (leftGlove.transform.parent == transform && rightGlove.transform.parent == transform && safetyGoggles.gameObject == null) {
            safetyOn = true;
        }
    }
    private void WearGoggles() {
        if (((Vector3.Distance(rightGlove.transform.position, camera.transform.position) < 0.20f && safetyGoggles.gameObject.transform.parent == rightGlove.transform) 
            || Vector3.Distance(leftGlove.transform.position, camera.transform.position) < 0.20f && safetyGoggles.gameObject.transform.parent == leftGlove.transform)) {
            safetyGogglesDone = true;
            safetyGoggles.transform.parent = null;
            safetyGoggles.transform.position = new Vector3(100, 100, 100);
        }
    }
}
