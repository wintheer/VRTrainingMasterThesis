using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.Diagnostics;
using UnityEngine.XR;

public class SceneLevelManager : MonoBehaviour {
    private bool calledOnce = false;
    private string sceneName;
    private Scene currentScene;
    private int sceneNumber;
    public bool donePlacingScrews = false;
    private PersistenceManager persistenceManager;
    private SubTask subTask;
    private bool allScenesAreDone = false;
    private float distToHint = 0.2f;
    private GameObject torqueScrewDriver;
    private GameObject cameraPlaceholder;

    //steps
    private GameObject canvas;

    private string stepListToggleButtonLeftController = "joystick 1 button 5";
    private string stepListToggleButtonRightController = "joystick 2 button 4";
    public bool showingText = false;
    //public int previousScene;

    private readonly string grabTag = "Grab";
    private readonly string screwTag = "Screw";
    private readonly string unTag = "Untagged";
    private Stopwatch stopwatch;
    public int timeToWait = 1000; //ms

    //private bool toggleKeyActivated = false;

    #region Sequence steps
    private bool performed1, performed2, performed3, performed4;
    private bool tightened1, tightened2, tightened3, tightened4;
    private bool searchedForObjects;

    private GameObject rightGrip, leftGrip, pressureReleaser;
    private GameObject topPlug, bottomPlug;
    private GameObject backLid;
    private GameObject screw1, screw2, screw3, screw4;
    private GameObject housing, diaphragm;
    private GameObject drainer, oRingOld, oRingNew;
    private GameObject diaphragmMovement;

    #region Scene 5

    #endregion
    private GameObject frontLidRotateObject, frontLid, buttonsPart;
    #endregion

    //Hints
    private GameObject hint1, hint2, hint3, hint4, hint5;
    private List<GameObject> hints;

    // Hands
    private GameObject leftHand, rightHand;

    // Human hands and gloves
    private GameObject humanCameraPlaceholder, leftGlove, rightGlove;

    // Goggles
    private GameObject safetyGoggles;

    // Sound
    private GameObject sound;

    void Start() {
        
        FindRelevantObjects();
        stopwatch = new Stopwatch();
        canvas.SetActive(false);
        persistenceManager = FindObjectOfType<PersistenceManager>();
        subTask = FindObjectOfType<SubTask>();
        hints = new List<GameObject>() { hint1, hint2, hint3, hint4, hint5 };
        foreach (GameObject h in hints) {
            if (h != null) {
                h.SetActive(false);
            }
        }
    }

    void Update() {
        if (Input.GetKeyDown(stepListToggleButtonLeftController) || Input.GetKeyDown(stepListToggleButtonRightController))
        {
            if (showingText == false) {
                canvas.SetActive(true);
                showingText = true;
            } else {
                canvas.SetActive(false);
                showingText = false;
            }
        }
        ManageStepSequence();
        if(allScenesAreDone == true && calledOnce == false){
            calledOnce = true;
            saveData(getCurrentSceneNumber());
        }
    }

    public void LoadScene(int i) {
        SceneManager.LoadScene("Scene" + i);
    }

    public int getCurrentSceneNumber()
    {
        currentScene = SceneManager.GetActiveScene();
        sceneName = currentScene.name;
        string[] lines = Regex.Split(sceneName, "Scene");
        sceneNumber = Convert.ToInt32(lines[1]);
        return sceneNumber;
    }

    private void saveData(int scene){
        persistenceManager.GetComponent<PersistenceManager>().setTimeForScene(scene);
        persistenceManager.GetComponent<PersistenceManager>().setAttemptForScene(scene);
        persistenceManager.GetComponent<PersistenceManager>().endSession();
        persistenceManager.GetComponent<PersistenceManager>().Save();
    }
    private void hintController(GameObject hint){
        // Only show hints if the difficulty is set to beginner!
        if (persistenceManager != null) {
            if (persistenceManager.difficultyNiveau.Equals(PersistenceManager.Difficulty.Beginner)) {
                float distLeftHand = Vector3.Distance(leftHand.transform.position, hint.transform.position);
                float distRightHand = Vector3.Distance(rightHand.transform.position, hint.transform.position);
                hint.SetActive(true);
                if (distLeftHand < distToHint || distRightHand < distToHint) {
                    hint.GetComponent<TransparentObject>().makeTransparent(0.005f);
                } else {
                    hint.GetComponent<TransparentObject>().makeTransparent(0.5f);
                }
            }
        }
    }

    private void ManageStepSequence() {

        switch (getCurrentSceneNumber()) {
            #region precautions 
            case -1:
                sound.GetComponent<PlaySound>().Speak(1);
                humanCameraPlaceholder = GameObject.Find("HumanCameraPlaceholder");
                leftGlove = GameObject.Find("LeftHandSkinnedMesh");
                rightGlove = GameObject.Find("RightHandSkinnedMesh");
                safetyGoggles = GameObject.Find("SafetyGoggles");
                if (!performed1) {
                    subTask.highlightUIText(2);
                    leftGlove.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    if (humanCameraPlaceholder.GetComponent<Gloves>().leftGloveDone) {
                        performed1 = true;
                    }
                }
                else if (!performed2) {
                    subTask.highlightUIText(3);
                    leftGlove.GetComponent<GlowObjectCmd>().shouldGlow = false;
                    rightGlove.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    if (humanCameraPlaceholder.GetComponent<Gloves>().rightGloveDone) {
                        performed2 = true;
                    }
                } 
                else if (!performed3) {
                    sound.GetComponent<PlaySound>().Speak(2);
                    subTask.highlightUIText(4);
                    rightGlove.GetComponent<GlowObjectCmd>().shouldGlow = false;
                    if (!humanCameraPlaceholder.GetComponent<Gloves>().safetyGogglesDone) {
                        safetyGoggles.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    }
                    if (humanCameraPlaceholder.GetComponent<Gloves>().safetyGogglesDone) {
                        sound.GetComponent<PlaySound>().Speak(3);
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed3 = true;
                            LoadScene(0);
                        }
                    }
                }
                break;
            case 0:
                sound.GetComponent<PlaySound>().Speak(1);
                if (!performed1) {
                    subTask.highlightUIText(1);
                    if (!searchedForObjects) {
                        searchedForObjects = true;
                        rightGrip = GameObject.Find("/PressureReleaser/R_Grip");
                        leftGrip = GameObject.Find("/PressureReleaser/L_Grip");
                        pressureReleaser = GameObject.Find("/PressureReleaser");
                    }

                    leftGrip.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    rightGrip.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);
                    hintController(hint2);
                    if (pressureReleaser.GetComponent<ReleasePressure>().IsDone) {
                        leftGrip.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        rightGrip.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        hint1.SetActive(false);
                        hint2.SetActive(false);
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed3 = true;
                            LoadScene(1);
                        }
                    }
                }
                break;
            #endregion
            #region Disassembly of pump
            case 1:
                // Top valve needs to be removed first
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    
                    topPlug.GetComponent<SnapToPlace>().ShouldActivate = true;
                    topPlug.GetComponent<HandLoosen>().shouldActivate = true;
                    topPlug.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Disassembly;
                    topPlug.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);

                    // Is it screwed off? Then we can pick it up!
                    if (topPlug.GetComponent<HandLoosen>().doneRotating) {
                        topPlug.tag = grabTag;
                        topPlug.GetComponent<HandLoosen>().shouldActivate = false;
                    }

                    // Has the user finished disassembling and placing the object?
                    if (topPlug.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        topPlug.tag = unTag;
                        performed1 = true;
                        topPlug.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        hint1.SetActive(false);
                    }

                } else if (!performed2) {
                    sound.GetComponent<PlaySound>().Speak(2);
                    subTask.highlightUIText(2);
                    bottomPlug.GetComponent<SnapToPlace>().ShouldActivate = true;
                    bottomPlug.GetComponent<HandLoosen>().shouldActivate = true;
                    bottomPlug.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Disassembly;
                    bottomPlug.GetComponent<GlowObjectCmd>().shouldGlow = true;

                    // Is it screwed off? Then we can pick it up!
                    if (bottomPlug.GetComponent<HandLoosen>().doneRotating) {
                        bottomPlug.tag = grabTag;
                        bottomPlug.GetComponent<HandLoosen>().shouldActivate = false;
                    }

                    // Has the user finished disassembling and placing the object?
                    if (bottomPlug.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        bottomPlug.tag = unTag;
                        bottomPlug.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed2 = true;
                            saveData(getCurrentSceneNumber());
                            LoadScene(2);
                        }
                    }
                }

                break;

            case 2:
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    backLid.tag = grabTag;
                    backLid.GetComponent<SnapToPlace>().ShouldActivate = true;
                    backLid.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Disassembly;
                    backLid.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);

                    // Has the user finished disassembling and placing the object?
                    if (backLid.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        backLid.tag = unTag;
                        backLid.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        hint1.SetActive(false);
                        // Wait 5 seconds
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed1 = true;
                            saveData(getCurrentSceneNumber());
                            LoadScene(3);
                        }
                    }
                }

                break;

            case 3:
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    screw1.GetComponent<SnapToPlace>().ShouldActivate = true;
                    screw1.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Disassembly;
                    screw1.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);

                    screw1.GetComponent<TightenAction>().shouldActivate = true;

                    if (screw1.GetComponent<TightenAction>().doneTightening) {
                        screw1.tag = screwTag;
                        screw1.GetComponent<TightenAction>().shouldActivate = false;
                    }

                    if (screw1.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        performed1 = true;
                        screw1.tag = unTag;
                        screw1.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        hint1.SetActive(false);
                    }
                } else if (!performed2) {
                    subTask.highlightUIText(2);
                    screw2.GetComponent<SnapToPlace>().ShouldActivate = true;
                    screw2.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Disassembly;
                    screw2.GetComponent<GlowObjectCmd>().shouldGlow = true;

                    screw2.GetComponent<TightenAction>().shouldActivate = true;

                    if (screw2.GetComponent<TightenAction>().doneTightening) {
                        screw2.tag = screwTag;
                        screw2.GetComponent<TightenAction>().shouldActivate = false;
                    }

                    if (screw2.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        performed2 = true;
                        screw2.tag = unTag;
                        screw2.GetComponent<GlowObjectCmd>().shouldGlow = false;
                    }
                } else if (!performed3) {
                    subTask.highlightUIText(3);
                    screw3.GetComponent<SnapToPlace>().ShouldActivate = true;
                    screw3.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Disassembly;
                    screw3.GetComponent<GlowObjectCmd>().shouldGlow = true;

                    screw3.GetComponent<TightenAction>().shouldActivate = true;

                    if (screw3.GetComponent<TightenAction>().doneTightening) {
                        screw3.tag = screwTag;
                        screw3.GetComponent<TightenAction>().shouldActivate = false;
                    }

                    if (screw3.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        performed3 = true;
                        screw3.tag = unTag;
                        screw3.GetComponent<GlowObjectCmd>().shouldGlow = false;
                    }
                } else if (!performed4) {
                    subTask.highlightUIText(4);
                    screw4.GetComponent<SnapToPlace>().ShouldActivate = true;
                    screw4.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Disassembly;
                    screw4.GetComponent<GlowObjectCmd>().shouldGlow = true;

                    screw4.GetComponent<TightenAction>().shouldActivate = true;

                    if (screw4.GetComponent<TightenAction>().doneTightening) {
                        screw4.tag = screwTag;
                        screw4.GetComponent<TightenAction>().shouldActivate = false;
                    }

                    if (screw4.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        screw4.tag = unTag;
                        screw4.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        // Time to load the next scene!
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed4 = true;
                            saveData(getCurrentSceneNumber());
                            LoadScene(4);
                        }
                    }
                }
                break;

            case 4:
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    housing.tag = grabTag;
                    housing.GetComponent<SnapToPlace>().ShouldActivate = true;
                    housing.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Disassembly;
                    housing.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);

                    if (housing.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        housing.tag = unTag;
                        housing.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        hint1.SetActive(false);
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed1 = true;
                            saveData(getCurrentSceneNumber());
                            LoadScene(5);
                        }
                    }
                }
                break;

            case 5: // THIS NEEDS GLOW ON BUTTONS!!!!
                if (frontLidRotateObject == null) {
                    frontLidRotateObject = GameObject.Find("FrontLid_trigger");
                }
                sound.GetComponent<PlaySound>().Speak(1);
                if (!performed1) {
                    if(sound.GetComponent<PlaySound>().isSpeaking() == false){
                        sound.GetComponent<PlaySound>().Speak(2);
                    }
                    subTask.highlightUIText(1);
                    frontLid.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);
                    if (frontLidRotateObject.GetComponent<RotatingFunctionality>().hasBeenRotated){
                        frontLid.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        performed1 = true;
                        hint1.SetActive(false);
                    }
                } else if (!performed2) {
                    sound.GetComponent<PlaySound>().Speak(3);
                    subTask.highlightUIText(2);
                    hintController(hint2);
                    if (frontLidRotateObject.GetComponent<RotatingFunctionality>().animationHasFinished) {
                        // This scene is done, time to lead scene 6!
                        hint2.SetActive(false);
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed2 = true;
                            saveData(getCurrentSceneNumber());
                            LoadScene(6);
                        }
                    }
                }
                break;

            case 6:
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    diaphragm.GetComponent<SnapToPlace>().ShouldActivate = true;
                    diaphragm.GetComponent<HandLoosen>().shouldActivate = true;
                    diaphragm.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Disassembly;
                    diaphragm.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);

                    if (diaphragm.GetComponent<HandLoosen>().doneRotating) {
                        diaphragm.tag = grabTag;
                    }

                    if (diaphragm.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        diaphragm.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        diaphragm.tag = unTag;
                        hint1.SetActive(false);
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed1 = true;
                            saveData(getCurrentSceneNumber());
                            LoadScene(7);
                        }
                    }
                }
                break;

            case 7:
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    drainer.tag = grabTag;
                    drainer.GetComponent<SnapToPlace>().ShouldActivate = true;
                    drainer.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Disassembly;
                    drainer.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);

                    if (drainer.GetComponent<SnapToPlace>().HasBeenSnapped)
                    {
                        drainer.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        drainer.tag = unTag;
                        hint1.SetActive(false);
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed1 = true;
                            saveData(getCurrentSceneNumber());
                            LoadScene(8);
                        }
                    }
                }
                break;

            case 8:
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    oRingOld.tag = screwTag;
                    oRingOld.GetComponent<SnapToPlace>().ShouldActivate = true;
                    oRingOld.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Disassembly;
                    oRingOld.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);

                    if (oRingOld.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        oRingOld.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        oRingOld.tag = unTag;
                        hint1.SetActive(false);
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed1 = true;
                            saveData(getCurrentSceneNumber());
                            LoadScene(9);
                        }
                    }
                }
                break;
            #endregion
            #region Assembly of pump
            case 9:
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    oRingNew.tag = grabTag;
                    oRingNew.GetComponent<SnapToPlace>().ShouldActivate = true;
                    oRingNew.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Assembly;
                    oRingNew.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);

                    if (oRingNew.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        oRingNew.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        oRingNew.tag = unTag;
                        hint1.SetActive(false);
                        oRingNew.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed1 = true;
                            saveData(getCurrentSceneNumber());
                            LoadScene(10);
                        }
                    }
                }
                break;

            case 10:
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    drainer.tag = grabTag;
                    drainer.GetComponent<SnapToPlace>().ShouldActivate = true;
                    drainer.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Assembly;
                    drainer.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);

                    if (drainer.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        drainer.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        drainer.tag = unTag;
                        hint1.SetActive(false);
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed1 = true;
                            saveData(getCurrentSceneNumber());
                            LoadScene(11);
                        }
                    }
                }
                break;

            case 11:
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    diaphragm.tag = grabTag;
                    diaphragm.GetComponent<SnapToPlace>().ShouldActivate = true;
                    diaphragm.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Assembly;
                    diaphragm.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);

                    if (diaphragm.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        diaphragm.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        diaphragm.tag = unTag;
                        hint1.SetActive(false);

                        diaphragm.GetComponent<HandLoosen>().shouldActivate = true;
                        diaphragm.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                        if (diaphragm.GetComponent<HandLoosen>().doneRotating) {
                            diaphragm.GetComponent<HandLoosen>().shouldActivate = false;
                            stopwatch.Start();
                            if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                                // Load next scene.
                                stopwatch.Stop();
                                performed1 = true;
                                saveData(getCurrentSceneNumber());
                                LoadScene(12);
                            }
                        }
                    }
                }
                break;

            case 12:

                if (diaphragmMovement == null) {
                    diaphragmMovement = GameObject.Find("DiaphragmMovement");
                }
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    hintController(hint1);
                    if (diaphragmMovement.GetComponent<CloseDiaphragm>().animationHasFinished) {
                        hint1.SetActive(false);
                        // This scene is done, time to lead scene 6!
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed2 = true;
                            saveData(getCurrentSceneNumber());
                            LoadScene(13);
                        }
                    }
                }
                break;

            case 13:
                sound.GetComponent<PlaySound>().Speak(1);
                subTask.highlightUIText(1);
                housing.tag = grabTag;
                housing.GetComponent<SnapToPlace>().ShouldActivate = true;
                housing.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Assembly;
                housing.GetComponent<GlowObjectCmd>().shouldGlow = true;
                hintController(hint1);
                if (housing.GetComponent<SnapToPlace>().HasBeenSnapped)
                {
                    housing.GetComponent<GlowObjectCmd>().shouldGlow = false;
                    housing.tag = unTag;
                    hint1.SetActive(false);
                    stopwatch.Start();
                    if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                        // Load next scene.
                        stopwatch.Stop();
                        performed1 = true;
                        saveData(getCurrentSceneNumber());
                        LoadScene(14);
                    }
                }

                break;

            case 14:
                #region Placing screws
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    screw1.tag = grabTag;
                    screw1.GetComponent<SnapToPlace>().ShouldActivate = true;
                    screw1.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Assembly;
                    screw1.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);
                    if (screw1.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        screw1.tag = unTag;
                        hint1.SetActive(false);
                        screw1.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                        screw1.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        performed1 = true;
                    }

                } else if (!performed2) {
                    subTask.highlightUIText(2);
                    screw2.tag = grabTag;
                    screw2.GetComponent<SnapToPlace>().ShouldActivate = true;
                    screw2.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Assembly;
                    screw2.GetComponent<GlowObjectCmd>().shouldGlow = true;

                    if (screw2.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        screw2.tag = unTag;
                        screw2.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                        screw2.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        performed2 = true;
                    }

                } else if (!performed3) {
                    subTask.highlightUIText(3);
                    screw3.tag = grabTag;
                    screw3.GetComponent<SnapToPlace>().ShouldActivate = true;
                    screw3.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Assembly;
                    screw3.GetComponent<GlowObjectCmd>().shouldGlow = true;

                    if (screw3.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        screw3.tag = unTag;
                        screw3.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                        screw3.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        performed3 = true;
                    }

                } else if (!performed4) {
                    subTask.highlightUIText(4);
                    screw4.tag = grabTag;
                    screw4.GetComponent<SnapToPlace>().ShouldActivate = true;
                    screw4.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Assembly;
                    screw4.GetComponent<GlowObjectCmd>().shouldGlow = true;

                    if (screw4.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        donePlacingScrews = true;
                        screw4.tag = unTag;
                        screw4.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
                        screw4.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        performed4 = true;
                    }
                }
                
                #endregion

                #region Tightening screws

                else if (!tightened1) {
                    // Screw 1
                    sound.GetComponent<PlaySound>().Speak(2);
                    subTask.highlightUIText(1);
                    screw1.GetComponent<TightenAction>().shouldActivate = true;
                    screw1.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    screw1.GetComponent<GlowObjectCmd>().glowSpecificColor(new Color(0.850f, 1.0f, 0f,1.0f));
                    hintController(hint2);
                    if (screw1.GetComponent<TightenAction>().doneTightening) {
                        screw1.GetComponent<SnapToPlace>().ShouldActivate = false;
                        screw1.GetComponent<TightenAction>().shouldActivate = false;
                        screw1.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        hint2.SetActive(false);
                        tightened1 = true;
                    }

                } else if (!tightened2) {
                    // Screw 4
                    subTask.highlightUIText(2);
                    screw4.GetComponent<TightenAction>().shouldActivate = true;
                    screw4.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    screw4.GetComponent<GlowObjectCmd>().glowSpecificColor(new Color(0.850f, 1.0f, 0f, 1.0f));
                    hintController(hint5);
                    if (screw4.GetComponent<TightenAction>().doneTightening)
                    {
                        screw4.GetComponent<SnapToPlace>().ShouldActivate = false;
                        screw4.GetComponent<TightenAction>().shouldActivate = false;
                        screw4.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        hint5.SetActive(false);
                        tightened2 = true;
                    }


                } else if (!tightened3) {
                    // Screw 2
                    subTask.highlightUIText(3);
                    screw2.GetComponent<TightenAction>().shouldActivate = true;
                    screw2.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    screw2.GetComponent<GlowObjectCmd>().glowSpecificColor(new Color(0.850f, 1.0f, 0f, 1.0f));
                    hintController(hint3);
                    if (screw2.GetComponent<TightenAction>().doneTightening) {
                        screw2.GetComponent<SnapToPlace>().ShouldActivate = false;
                        screw2.GetComponent<TightenAction>().shouldActivate = false;
                        screw2.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        hint3.SetActive(false);
                        tightened3 = true;
                    }

                } else if (!tightened4) {
                    // Screw 3
                    subTask.highlightUIText(4);
                    screw3.GetComponent<TightenAction>().shouldActivate = true;
                    screw3.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    screw3.GetComponent<GlowObjectCmd>().glowSpecificColor(new Color(0.850f, 1.0f, 0f, 1.0f));
                    hintController(hint4);
                    if (screw3.GetComponent<TightenAction>().doneTightening) {
                        screw3.GetComponent<TightenAction>().shouldActivate = false;
                        screw3.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        hint4.SetActive(false);
                        // Done tightening screws when this is done!
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            screw3.GetComponent<SnapToPlace>().ShouldActivate = false;
                            // Load next scene.
                            stopwatch.Stop();
                            tightened4 = true;
                            saveData(getCurrentSceneNumber());
                            LoadScene(15);
                        }
                    }
                }

                #endregion
                break;

            case 15:
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    backLid.tag = grabTag;
                    backLid.GetComponent<SnapToPlace>().ShouldActivate = true;
                    backLid.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Assembly;
                    backLid.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);
                    if (backLid.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        hint1.SetActive(false);
                        backLid.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        //Time to load the last scene!
                        stopwatch.Start();
                        if (stopwatch.ElapsedMilliseconds >= timeToWait && !sound.GetComponent<PlaySound>().isSpeaking()) {
                            // Load next scene.
                            stopwatch.Stop();
                            performed1 = true;
                            saveData(getCurrentSceneNumber());
                            LoadScene(16);
                        }
                    }
                }
                break;

            case 16:
                if (!performed1) {
                    sound.GetComponent<PlaySound>().Speak(1);
                    subTask.highlightUIText(1);
                    bottomPlug.tag = grabTag;
                    bottomPlug.GetComponent<SnapToPlace>().ShouldActivate = true;
                    bottomPlug.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Assembly;
                    bottomPlug.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint1);
                    if (bottomPlug.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        hint1.SetActive(false);
                        bottomPlug.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        bottomPlug.tag = unTag;
                        bottomPlug.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

                        // Time To tighten the plug!
                        bottomPlug.GetComponent<HandLoosen>().shouldActivate = true;

                        if (bottomPlug.GetComponent<HandLoosen>().doneRotating) {
                            performed1 = true;
                            bottomPlug.GetComponent<HandLoosen>().shouldActivate = false;
                        }
                    }

                } else if (!performed2) {
                    sound.GetComponent<PlaySound>().Speak(2);
                    subTask.highlightUIText(2);
                    topPlug.tag = grabTag;
                    topPlug.GetComponent<SnapToPlace>().ShouldActivate = true;
                    topPlug.GetComponent<SnapToPlace>().WhichState = SnapToPlace.State.Assembly;
                    topPlug.GetComponent<GlowObjectCmd>().shouldGlow = true;
                    hintController(hint2);
                    if (topPlug.GetComponent<SnapToPlace>().HasBeenSnapped) {
                        hint2.SetActive(false);
                        topPlug.tag = unTag;
                        topPlug.GetComponent<GlowObjectCmd>().shouldGlow = false;
                        topPlug.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;

                        // Time To tighten the plug!
                        topPlug.GetComponent<HandLoosen>().shouldActivate = true;

                        if (topPlug.GetComponent<HandLoosen>().doneRotating) {
                            performed2 = true;
                            topPlug.GetComponent<HandLoosen>().shouldActivate = false;
                            // It is now finished!
                            sound.GetComponent<PlaySound>().Speak(3);
                            allScenesAreDone = true;
                        }
                    }
                }
                break;

                #endregion
        }
    }


    private void FindRelevantObjects() {

        canvas = GameObject.Find("CanvasPlaceholder/Canvas");

        topPlug = GameObject.Find("/DetailedPump2/TopPlug");
        bottomPlug = GameObject.Find("/DetailedPump2/BottomPlug");
        backLid = GameObject.Find("/DetailedPump2/BackLid");

        screw1 = GameObject.Find("/DetailedPump2/Screw1");
        screw2 = GameObject.Find("/DetailedPump2/Screw2");
        screw3 = GameObject.Find("/DetailedPump2/Screw3");
        screw4 = GameObject.Find("/DetailedPump2/Screw4");

        torqueScrewDriver = GameObject.Find("/TorquePlaceholder/Torque");

        housing = GameObject.Find("/DetailedPump2/RearHousing");

        diaphragm = GameObject.Find("/DetailedPump2/Diaphragm");

        drainer = GameObject.Find("/DetailedPump2/Drainer");

        oRingOld = GameObject.Find("/DetailedPump2/Drainer/ORing");
        oRingNew = GameObject.Find("ORingNew");
        
        hint1 = GameObject.Find("Hint1");
        hint2 = GameObject.Find("Hint2");
        hint3 = GameObject.Find("Hint3");
        hint4 = GameObject.Find("Hint4");
        hint5 = GameObject.Find("Hint5");

        leftHand = GameObject.Find("ToolRefLeft");
        rightHand = GameObject.Find("ToolRefRight");

        frontLid = GameObject.Find("FrontLid");
        buttonsPart = GameObject.Find("Body1 28");

        sound = GameObject.Find("Speech");
    }
}
