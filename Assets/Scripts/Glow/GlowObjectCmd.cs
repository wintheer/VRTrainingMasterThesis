using UnityEngine;
using System.Collections.Generic;

public class GlowObjectCmd : MonoBehaviour
{
    private Color GlowColor = Color.yellow;
    public float LerpFactor = 10;

    public bool shouldGlow { get; set; }
    private bool partyEngaged = false;
    private GameObject persistanceObj;
    private PersistenceManager persistance;
    public bool doneChangingColor;
    private bool byPass = false;

    public Renderer[] Renderers
    {
        get;
        private set;
    }

    public Color CurrentColor
    {
        get { return _currentColor; }
    }

    private Color _currentColor;
    private Color _targetColor;

    void Start() {
        Renderers = GetComponentsInChildren<Renderer>();
        GlowController.RegisterObject(this);
        persistanceObj = GameObject.Find("/PersistenceManager");
        
        if (persistanceObj == null) {
            byPass = true;
        } else {
            persistance = persistanceObj.GetComponent<PersistenceManager>();
        }
    }

    private void Update() {

        if (persistance != null) {
            if (shouldGlow && persistance.difficultyNiveau.Equals(PersistenceManager.Difficulty.Beginner)) {
                _targetColor = GlowColor;
                enabled = true;
            } else {
                _targetColor = Color.black;
                enabled = true;
            }
        } else {
            if (shouldGlow && byPass) {
                _targetColor = GlowColor;
                enabled = true;
            } else {
                _targetColor = Color.black;
                enabled = true;
            }
        }

        if (_currentColor.Equals(_targetColor) == false) {
            //_currentColor = Color.Lerp(_currentColor, _targetColor, Time.deltaTime * LerpFactor);
            _currentColor = _targetColor;
        } 
    }
    public void glowSpecificColor(Color c){
        GlowColor = c;
    }
    public void standardColor(){
        GlowColor = Color.yellow;
    }
}
