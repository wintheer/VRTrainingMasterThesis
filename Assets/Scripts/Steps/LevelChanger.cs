using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LevelChanger : MonoBehaviour
{

    private Scene1Steps scene1;
    private GameObject backScrew;

    private Scene2Steps scene2;
    private GameObject backLid;

    private int level = 1;

    public int maxLevel = 4;

    Scene activeScene;
    string sceneName;

    void Awake()
    {/*
        activeScene = SceneManager.GetActiveScene();
        sceneName = activeScene.name;
        if (sceneName != "Scene1")
        {
            if (this.name != "GameController")
            {
                DontDestroyOnLoad(this);
            }
        }*/

    }

    void Start()
    {
        Debug.Log(this.name);
        scene1 = new Scene1Steps();
        backScrew = GameObject.Find("Back_screw");

        scene2 = new Scene2Steps();
        backLid = GameObject.Find("BackLid");
    }

    void Update()
    {

        //Condition to go up one level 
        if (Input.GetKeyDown("+"))
        {
            level = level + 1;
            GotoLevel(level);
        }
        else if (Input.GetKeyDown("-"))
        {
            level = level - 1;
            GotoLevel(level);
        }
        if (Input.GetKeyDown("a"))
        {
            Action(level);
        }
    }

    public void GotoLevel(int i)
    {
        //load scene based on num
        Debug.Log("Now loading level: " + level);
        SceneManager.LoadScene("Scene" + i);
    }

    public void Action(int i)
    {
        switch (i)
        {
            case 1:
                backScrew.AddComponent<BoxCollider>();
                backScrew.AddComponent<Rigidbody>();
                backScrew.GetComponent<Rigidbody>().useGravity = true;
                backScrew.GetComponent<Rigidbody>().AddTorque(1, 0, 0);
                break;
            case 2:
                backLid.AddComponent<BoxCollider>();
                backLid.AddComponent<Rigidbody>();
                backLid.GetComponent<Rigidbody>().useGravity = true;
                backLid.GetComponent<Rigidbody>().AddTorque(1, 0, 0);
                break;
            default:
                break;
        }
    }
}
