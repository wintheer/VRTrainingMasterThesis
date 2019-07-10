using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class StepController : MonoBehaviour
{
    //step 1
    public GameObject step1;
    public GameObject step1Target;
    private GameObject step1Instantiated;
    private GameObject step1TargetInstantiated;
    private GameObject backScrew;

    //step 2
    public GameObject step2;
    public GameObject step2Target;
    private GameObject step2Instantiated;
    private GameObject step2TargetInstantiated;
    private GameObject backLid;

    public int stepNumber;

    void Start()
    {
        backScrew = GameObject.Find("Back_screw");
        backLid = GameObject.Find("Back_lid");
    }

    void Update()
    {
        if (Input.GetKeyDown("p"))
        {
            NextStep();
        }
    }

    public void NextStep()
    {
        stepNumber++;
        DoStep(stepNumber);
    }

    private void DoStep(int i)
    {
        switch (i)
        {
            case 1:
                Debug.Log("STEP 1");
                step1Instantiated = Instantiate(step1);
                step1TargetInstantiated = Instantiate(step1Target);
                break;
            case 2:
                //when successfully unscrewed
                backScrew.GetComponent<Rigidbody>().useGravity = true;
                backScrew.GetComponent<Rigidbody>().AddTorque(1, 0, 0);
                StepDone();
                break;
            case 3:
                Debug.Log("STEP 2");
                //destroy elements from previous step
                Destroy(step1Instantiated);
                Destroy(step1TargetInstantiated);
                Destroy(backScrew);
                //initiate new objects
                step2Instantiated = Instantiate(step2);
                step2TargetInstantiated = Instantiate(step2Target);
                break;
            case 4:
                backLid.GetComponent<Rigidbody>().useGravity = true;
                backLid.GetComponent<Rigidbody>().AddTorque(0, 0, 1);
                break;
            case 5:
                Debug.Log("STEP 3");
                Destroy(step2Instantiated);
                Destroy(step2TargetInstantiated);
                Destroy(backLid);
                break;
            case 6:
                Debug.Log("STEP 4");
                break;
            default:
                Debug.Log("You have completed the training!");
                //Finish();
                break;
        }
    }

    public void StepDone()
    {
        Debug.Log("Step completed. Going to step: " + stepNumber);

    }
}