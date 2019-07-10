using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class Mission : MonoBehaviour
{
    public List<Operation> operations;
    public Text text1, text2, text3, text4, text5, text6, text7, text8, text9, text10, text11, text12, text13, text14, text15, text16;
    private List<Text> texts;
    public string InputName, trackpadLeft, trackpadRight;
    private string taskRightController = "joystick 1 button 9", taskLeftController = "joystick 2 button 9";
    private int currentTask;
    public GameObject allTexts;
    private SceneLevelManager manager;

    private bool menuIsOpen = false;


    // Start is called before the first frame update
    void Start()
    {
        // Initialization of all UI Texts
        texts = new List<Text>() { text1, text2, text3, text4, text5, text6, text7, text8, text9, text10, text11, text12, text13, text14, text15, text16 };
        operations = new List<Operation>();
        // Inserting all tasks in array
        operations.Add(new Operation("1. Dismantle the ball valves"));
        operations.Add(new Operation("2. Remove the backlid"));
        operations.Add(new Operation("3. Remove the screws"));
        operations.Add(new Operation("4. Remove the dosing head"));
        operations.Add(new Operation("5. Move the diaphragm forward"));
        operations.Add(new Operation("6. Remove the diaphragm"));
        operations.Add(new Operation("7. Remove the drainer"));
        operations.Add(new Operation("8. Remove the worn out o-ring"));
        operations.Add(new Operation("9. Assemble the new o-ring"));
        operations.Add(new Operation("10. Attach the drainer"));
        operations.Add(new Operation("11. Attach the diaphragm"));
        operations.Add(new Operation("12. Move the diaphragm backwards"));
        operations.Add(new Operation("13. Mount the dosing head"));
        operations.Add(new Operation("14. Place and tighten the screws"));
        operations.Add(new Operation("15. Attach the backlid"));
        operations.Add(new Operation("16. Assemble the ball valves"));

        for (int i = 0; i < 16; i++) {
            texts[i].text = operations[i].getName();
        }
        manager = FindObjectOfType<SceneLevelManager>();
        setCurrentTask(manager.GetComponent<SceneLevelManager>().getCurrentSceneNumber());
        focusOnTaskInStepList(manager.GetComponent<SceneLevelManager>().getCurrentSceneNumber());
    }

    // Update is called once per frame
    void Update()
    {
        //for (int i = 0; i < 20; i++) {
        //    if (Input.GetKeyDown("joystick 1 button " + i)) {
        //        print("joystick 1 button " + i);
        //    }
        //}
        if ((Input.GetAxis(trackpadLeft) == 1.0f && Input.GetKeyDown(taskLeftController)) || (Input.GetAxis(trackpadRight) == 1.0f && Input.GetKeyDown(taskRightController))) {
            // Click at the bottom part of the trackpad
            if (getCurrentTask() <= 15) {
                nextTask();
                focusOnTaskInStepList(getCurrentTask());
                
            }
        }
        if (Input.GetAxis(trackpadLeft) == -1.0f && Input.GetKeyDown(taskLeftController) || (Input.GetAxis(trackpadRight) == -1.0f && Input.GetKeyDown(taskRightController))) {
            // Click at the top part of the trackpad
            if (getCurrentTask() >= 2) {
                previousTask();
                focusOnTaskInStepList(getCurrentTask());
            }
        }
        if (Input.GetAxis(trackpadRight) > -0.95f && Input.GetAxis(trackpadRight) < 0.95f && (Input.GetKeyDown(taskRightController) || Input.GetKeyDown(taskLeftController))) {
            // Click at the center of the trackpad
            manager.GetComponent<SceneLevelManager>().LoadScene(getCurrentTask());

        }

    }
    public void showTexts(bool status){
        // Show or hide all UI Text
        //GameObject.Find("Text" + number).SetActive(status);
        foreach (Text t in texts) {
            t.gameObject.SetActive(status);
        }
    }
    public void showSpecificUIText(int number, bool status) {
        // Show a specific UI Text
        Text specificText = texts.Find(obj => obj.name == "Text"+number);
        specificText.gameObject.SetActive(status);
    }
    public int getCurrentTask() {
        // To get the current task/scene
        return currentTask;
    }
    public void setCurrentTask(int task) {
        // To set the current task/scene
        currentTask = task;
    }
    public void nextTask(){
        currentTask += 1;
    }
    public void previousTask(){
        currentTask -= 1;
    }
    public void clearColor() {
        // Make all UI Texts white
        Color chosenColor = Color.white;
        foreach (Text t in texts) {
            t.color = chosenColor;
        }
    }
    public void focusOnTaskInStepList(int t) {
        if (t > 0) {
            // Setting color and position of all UI Texts
            int upperLimit = texts.Count - 3;
            float startPosition = 0.3f;
            float multiplyFactor = 13.5f;
            clearColor();
            // Set the color of the UI Text of the current task to orange
            Text textToFocus = texts.Find(obj => obj.name == "Text" + t);
            textToFocus.color = new Color(255, 145, 0);
            // Setting the position of all UI Texts
            if (t == 4) {
                allTexts.transform.localPosition = new Vector3(0.3f, 0.3f, 5.9f);
            } 
            else if (t >= 5 && t <= upperLimit) {
                int dif = t - 4;
                allTexts.transform.localPosition = new Vector3(allTexts.transform.localPosition.x, startPosition + (multiplyFactor * dif), allTexts.transform.localPosition.z);
            }
            else if(t > upperLimit){
                allTexts.transform.localPosition = new Vector3(0.3f, 121.8f, 5.9f);
            }
        } 
        else {
            clearColor();
        }
    }
}