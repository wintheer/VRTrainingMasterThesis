using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using System.Text;
using UnityEditor;
using System.Text.RegularExpressions;
using System;

public class PersistenceManager : MonoBehaviour
{
    public static PersistenceManager Instance { get; private set; }

    public enum Difficulty { Beginner, Intermediate, }
    public Difficulty difficultyNiveau = Difficulty.Beginner;
    public float sessionIDTextField;
    private float sessionID;
    private float totalTime;
    private float timeScene1, timeScene2, timeScene3, timeScene4, timeScene5,
    timeScene6, timeScene7, timeScene8, timeScene9, timeScene10, timeScene11,
    timeScene12, timeScene13, timeScene14, timeScene15, timeScene16;
    private List<float> allTimes;
    private int attempsScene1, attempsScene2, attempsScene3, attempsScene4, attempsScene5,
    attempsScene6, attempsScene7, attempsScene8, attempsScene9, attempsScene10, attempsScene11,
    attempsScene12, attempsScene13, attempsScene14, attempsScene15, attempsScene16;
    private List<float> allAttemps;
    private SceneLevelManager manager;
    private List<string[]> rowData = new List<string[]>();
    StreamWriter sw;
    private GameObject torqueScrewdriver, needleTool;
    private Vector3 initialPosTorque, initialPosNeedle;
    private void Awake()
    {
        manager = FindObjectOfType<SceneLevelManager>();
        string[] lines = System.IO.File.ReadAllLines(@"Assets/Data/"+Environment.MachineName+".csv");
        var allString = new List<string>();
        torqueScrewdriver = GameObject.Find("TorquePlaceholder");
        needleTool = GameObject.Find("NeedleToolPlaceholder");
        initialPosTorque = torqueScrewdriver.transform.position;
        initialPosNeedle = needleTool.transform.position;
        sessionID = float.Parse(sessionIDTextField.ToString());
        //if (lines[0] != null && lines.Length > 1)
        //{
        //    // Access latest sessionID
        //    string[] splitLines = Regex.Split(lines[lines.Length - 1], ";");
        //    sessionID = float.Parse(sessionIDTextField.ToString());
        //}
        //else{
        //    sessionID = 1;
        //}
        //PlayerPrefs.SetInt("SessionID", PlayerPrefs.GetInt("SessionID") + 1);
        //sessionID = PlayerPrefs.GetInt("SessionID");
        allTimes = new List<float>() { timeScene1, timeScene2,timeScene3,timeScene4,timeScene5,
                    timeScene6,timeScene7,timeScene8,timeScene9,timeScene10,timeScene11,
                    timeScene12,timeScene13,timeScene14,timeScene15,timeScene16 };

        allAttemps = new List<float>() { attempsScene1, attempsScene2,attempsScene3,attempsScene4,attempsScene5,
                    attempsScene6,attempsScene7,attempsScene8,attempsScene9,attempsScene10,attempsScene11,
                    attempsScene12,attempsScene13,attempsScene14,attempsScene15,attempsScene16 };
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown("r")) {
            resetTools();
        }
    }
    public void setTimeForScene(int scene)
    {
        if (scene == 1)
        {
            allTimes[scene - 1] += (float)Math.Round(Time.time, 1);
        }
        else
        {
            allTimes[scene - 1] += (float)Math.Round(Time.timeSinceLevelLoad, 1);
        }

    }
    public void setAttemptForScene(int scene)
    {
        allAttemps[scene - 1] += 1;
    }
    public void endSession()
    {
        totalTime = (float)Math.Round(Time.time, 2);

    }
    public void Save()
    {
        // Saving all new/updated data to local List
        var sessionInfo = new List<float>();
        sessionInfo.Add(sessionID);
        sessionInfo.Add(totalTime);

        var allInfo = new List<float>();
        allInfo.AddRange(sessionInfo);
        allInfo.AddRange(allTimes);
        allInfo.AddRange(allAttemps);

        // Reading and saving all data in an array
        string[] lines = System.IO.File.ReadAllLines(@"Assets/Data/"+Environment.MachineName+".csv");
        float fetchedSessionID = 0f;
        var allString = new List<string>();
        if (lines[0] != null && lines.Length > 1)
        {
            // Access latest sessionID
            string[] splitLines = Regex.Split(lines[lines.Length - 1], ";");
            fetchedSessionID = float.Parse(splitLines[0]);

            // Accessing all row titles
            for (int i = 0; i < lines.Length; i++)
            {
                allString.Add(lines[i]);
            }
            // Writing all new/updated floats to one long string
            string allInfoString = "";
            for (int i = 0; i < allInfo.Count; i++)
            {
                if (i == allInfo.Count - 1)
                {
                    allInfoString += allInfo[i];
                }
                else
                {
                    allInfoString += allInfo[i] + "; ";
                }
            }
            // Adding the new long string to the fetched List
            if (fetchedSessionID == sessionID)
            {
                // If in current session replace the last string with updated values
                allString[allString.Count-1] = allInfoString;
            }
            else
            {
                // If new session add new string with new values
                allString.Add(allInfoString);
            }

            // Converting the float list to one long string
            string convertedToString = "";
            for (int j = 0; j < allString.Count; j++)
            {
                convertedToString += allString[j] + "\n";
            }
            // Updating data file
            System.IO.File.WriteAllText(@"Assets/Data/"+Environment.MachineName+".csv", convertedToString);
        }
        else if (lines.Length == 1) {
            // Accessing all row titles
            allString.Add(lines[0]);

            // Writing all new/updated floats to one long string
            string allInfoString = "";
            for (int i = 0; i < allInfo.Count; i++)
            {
                if (i == allInfo.Count - 1)
                {
                    allInfoString += allInfo[i];
                }
                else
                {
                    allInfoString += allInfo[i] + "; ";
                }
            }
            
            allString.Add(allInfoString);

            // Converting the float list to one long string
            string convertedToString = "";
            for (int j = 0; j < allString.Count; j++) {
                convertedToString += allString[j] + "\n";
            }
            // Updating data file
            //float newFileNumber = 10;
            System.IO.File.WriteAllText(@"Assets/Data/"+Environment.MachineName+".csv", convertedToString);
        }
        else{
            
        }
    }
    public void resetTools() {
        torqueScrewdriver.transform.position = initialPosTorque;
        needleTool.transform.position = initialPosNeedle;
    }
}