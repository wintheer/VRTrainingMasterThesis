using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{

    public AudioClip sound1;
    public AudioClip sound2;
    public AudioClip sound3;

    private AudioSource audioData;
    private bool oneHasPlayed;
    private bool twoHasPlayed;
    private bool threeHasPlayed;

    void Start()
    {
        audioData = GetComponent<AudioSource>();
        audioData.playOnAwake = false;
    }

    public void Speak(int i = 1) {
        if (!audioData.isPlaying) {
            if (i == 1 && !oneHasPlayed) {
                oneHasPlayed = true;
                audioData.PlayOneShot(sound1, 1f);
            } else if (i == 2 && !twoHasPlayed) {
                twoHasPlayed = true;
                audioData.PlayOneShot(sound2, 1f);
            } else if (i == 3 && !threeHasPlayed) {
                threeHasPlayed = true;
                audioData.PlayOneShot(sound3, 1f);
            }
        }
    }
    public bool isSpeaking(){
        if(audioData.isPlaying){
            return true;
        }
        else{
            return false;
        }
    }
}
