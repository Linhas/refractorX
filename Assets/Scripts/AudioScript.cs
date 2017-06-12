using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {

    public AudioSource[] audios;

    // Use this for initialization
    void Start () {
        AudioSource[] audios = GetComponents<AudioSource>();
    }

    void PlaySound()
    {
        audios[0].Play();
    }
	
	
}
