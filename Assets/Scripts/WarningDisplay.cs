using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarningDisplay : MonoBehaviour
{
    private GameObject display;
    private Color warningColor;
    private Material warningMaterial;
    private Color normalColor;
    private Material normalMaterial;
    private Renderer rend;
    private Material[] mats;
    private int arrayNo = 5;
    private bool warningOn = false;


    void Start()
    {

        display = GameObject.Find("Body1 90");
        warningColor = new Color(1.0f, 0.6f, 0.0f, 1.0f);
        rend = display.GetComponent<Renderer>();
        mats = rend.materials;
        normalMaterial = mats[arrayNo];
        warningMaterial = mats[arrayNo];
        normalColor = normalMaterial.color;
        //normalColor = new Color(1.0f, 0.2f, 0.1f, 1.0f);

        Warning();
    }

    public void Warning()
    {
        /*

        warningMaterial.color = normalColor;
        warningMaterial.color = warningColor;

        */

        //mats[arrayNo] = warningMaterial;
        display.GetComponent<Renderer>().materials = mats;

        StartCoroutine("Blink");
    }   

    IEnumerator Blink()
    {
        for (; ; )
        {
            if (warningOn)
            {
                warningMaterial.color = warningColor;
                display.GetComponent<Renderer>().materials = mats;
                warningOn = false;
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                warningMaterial.color = normalColor;
                display.GetComponent<Renderer>().materials = mats;
                warningOn = true;
                yield return new WaitForSeconds(0.5f);
            }
            //yield return null;
        }
    }
}
