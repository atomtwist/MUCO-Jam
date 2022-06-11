using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipRandomOffset : MonoBehaviour
{

    public bool randoPitch;
    public AudioSource audio;
    

    void Awake()
    {
        if(audio == null)
            audio = GetComponent<AudioSource>();
        var rnd = Random.Range(0,audio.clip.length);
        var rndPitch = Random.Range(-.3f, .3f);
        if (randoPitch)
            audio.pitch += rndPitch;
        audio.time = rnd;
        audio.Play();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
