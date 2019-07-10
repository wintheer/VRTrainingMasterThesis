using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectIsInsideSnapping : MonoBehaviour
{
    public int ObjectID; // Set this to the object ID that should snap to this point
    public bool IsTriggered = false;
    private Color initialColor;
    private GameObject sceneLevelManager;

    public void Start()
    {
        // If we forgot to set the object to be trigger this will do it for us.
        if (GetComponent<Collider>().isTrigger == false) {
            Debug.Log("GameObject: " + name + " didn't have isTrigger property. This was changed in Runtime. See script: ObjectIsInsideSnapping.");
            GetComponent<Collider>().isTrigger = true;
        }
        // Make all children transparent
        setChildrenTransparent();
        initialColor = getEmptyChildColor();
        sceneLevelManager = GameObject.Find("SceneLevelManager");
    }

    private void OnTriggerEnter(Collider other) {

        if (other.gameObject.GetComponent<SnapToPlace>().GetObjectID() == this.ObjectID) {
            other.gameObject.GetComponent<GlowObjectCmd>().glowSpecificColor(Color.green);
            childrenShaderColor(Color.green);
            IsTriggered = true;
        }
    }

    private void OnTriggerExit(Collider other) {

        if (other.gameObject.GetComponent<SnapToPlace>().GetObjectID() == this.ObjectID) {
            other.gameObject.GetComponent<GlowObjectCmd>().standardColor();
            childrenShaderColor(initialColor);
            IsTriggered = false;
        }
    }

    public bool HasObjectInside() {

        return IsTriggered;
    }

    private Color getEmptyChildColor() {
        var children = GetComponentsInChildren<Renderer>();

        foreach (var child in children)
        {
            if (child.GetComponent<Renderer>() != null) {
                return child.GetComponent<Renderer>().material.GetColor("_EmissionColor");
            }
        }

        return new Color(0,0,0,1);
    }

    private void setChildrenTransparent() {
        var children = GetComponentsInChildren<Renderer>();

        foreach (var child in children) {
            if (child.GetComponent<Renderer>() != null) {
                Color OriginalColor = child.GetComponent<Renderer>().material.color;
                child.GetComponent<Renderer>().material.color = new Color(OriginalColor.r, OriginalColor.g, OriginalColor.b, 0.5f);
            }
        }
    }
    private void childrenShaderColor(Color c)
    {
        //this.gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", c);
        var children = GetComponentsInChildren<Renderer>();
        foreach (var child in children)
        {
            if (child.GetComponent<Renderer>().materials.Length > 1) {
                Material[] mats = child.GetComponent<Renderer>().materials;
                foreach (var mat in mats) {
                    mat.SetColor("_EmissionColor", c);
                }
                child.GetComponent<Renderer>().materials = mats;
            }
            if (child.GetComponent<Renderer>().materials.Length == 1) {
                if (child.GetComponent<Renderer>().material != null) {
                    child.GetComponent<Renderer>().material.SetColor("_EmissionColor", c);
                }
            }
            
        }
    }
}
