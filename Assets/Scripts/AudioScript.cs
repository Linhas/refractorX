using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioScript : MonoBehaviour {

    public AudioClip[] audios;

    // Use this for initialization
    void Start () {
        AudioSource audio = GetComponent<AudioSource>();
        //AudioClip[] audios = GetComponents<AudioClip>();
    }

    void PlaySound(int v)
    {
        GetComponent<AudioSource>().PlayOneShot(audios[v], 5);
   //     audios[0].Play();
    }
    public void PlayFootsteps(int v)
    {
        GetComponent<AudioSource>().PlayOneShot(audios[Random.Range(v, audios.Length)], 1);
        //     audios[0].Play();
    }



}
