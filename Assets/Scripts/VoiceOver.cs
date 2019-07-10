using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceOver : MonoBehaviour
{
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update(){
        if(Input.GetKeyDown("s")){
            PlaySound();
        }
    }

    public void PlaySound(){
        if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
    }
}
