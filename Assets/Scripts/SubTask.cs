using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubTask : MonoBehaviour {
    int sceneNumber;
    private Text text1, text2, text3, text4;
    private List<Text> texts;
    //public Text text1, text2, text3, text4;
    private SceneLevelManager manager;
    // Start is called before the first frame update
    void Start() {
        manager = FindObjectOfType<SceneLevelManager>();
        FindRelevantUITexts();
        texts = new List<Text>() { text1, text2, text3, text4 };
        sceneNumber = manager.GetComponent<SceneLevelManager>().getCurrentSceneNumber();
        foreach (Text t in texts) {
            t.text = "";
        }
    }

    // Update is called once per frame
    void Update() {
        switch (sceneNumber) {
            case -1:
                text1.text = "Display shows warning \nTime to replace the o-ring";
                text2.text = "Put on the left glove";
                text3.text = "Put on the right glove";
                text4.text = "Put on the safety glasses";
                break;
            case 0:
                text1.text = "Turn the gribs \nto remove the pressure";
                break;
            case 1:
                text1.text = "Remove the top ball valve";
                text2.text = "Remove the bottom ball valve";
                break;
            case 2:
                text1.text = "Remove the backlid";
                break;
            case 3:
                text1.text = "Remove the first screw";
                text2.text = "Remove the second screw";
                text3.text = "Remove the third screw";
                text4.text = "Remove the fourth screw";
                break;
            case 4:
                text1.text = "Remove the dosing head";
                break;
            case 5:
                text1.text = "Open the frontlid";
                text2.text = "Press the two buttons at the same \ntime for 2 seconds";
                break;
            case 6:
                text1.text = "Unscrew and remove the diaphragm";
                break;
            case 7:
                text1.text = "Remove the drainer";
                break;
            case 8:
                text1.text = "Remove the worn out o-ring";
                break;
            case 9:
                text1.text = "Attach the new o-ring";
                break;
            case 10:
                text1.text = "Attach the drainer";
                break;
            case 11:
                text1.text = "Attach the diaphragm and screw it into place";
                break;
            case 12:
                text1.text = "Press the two buttons at the same \ntime for 2 seconds";
                break;
            case 13:
                text1.text = "Attach the dosing head";
                break;
            case 14:
                if (manager.GetComponent<SceneLevelManager>().donePlacingScrews == false) {
                    text1.text = "Place the first screw";
                    text2.text = "Place the second screw";
                    text3.text = "Place the third screw";
                    text4.text = "Place the fourth screw";
                }
                if (manager.GetComponent<SceneLevelManager>().donePlacingScrews == true) {
                    text1.text = "Tighten the first screw";
                    text2.text = "Tighten the fourth screw";
                    text3.text = "Tighten the second screw";
                    text4.text = "Tighten the third screw";
                }
                break;
            case 15:
                text1.text = "Attach the backlid";
                break;
            case 16:
                text2.text = "Attach and tighten \nthe top ball valve";
                text1.text = "Attach and tighten \nthe bottom ball valve";
                break;
            default:

                break;
        }
    }
    public void clearColor() {
        // Make all UI Texts white
        Color chosenColor = Color.white;
        foreach (Text t in texts) {
            t.color = chosenColor;
        }
    }
    public void highlightUIText(int UITextNumber) {
        clearColor();
        Text text = GameObject.Find("/iPadPlaceholder/iPad/ControliPad/ipadAir/Canvas/Everything/All_texts/SubText" + UITextNumber).GetComponent<Text>();
        text.color = new Color(255, 145, 0);
    }
    private void FindRelevantUITexts() {
        text1 = GameObject.Find("/iPadPlaceholder/iPad/ControliPad/ipadAir/Canvas/Everything/All_texts/SubText1").GetComponent<Text>();
        text2 = GameObject.Find("/iPadPlaceholder/iPad/ControliPad/ipadAir/Canvas/Everything/All_texts/SubText2").GetComponent<Text>();
        text3 = GameObject.Find("/iPadPlaceholder/iPad/ControliPad/ipadAir/Canvas/Everything/All_texts/SubText3").GetComponent<Text>();
        text4 = GameObject.Find("/iPadPlaceholder/iPad/ControliPad/ipadAir/Canvas/Everything/All_texts/SubText4").GetComponent<Text>();
    }
}
