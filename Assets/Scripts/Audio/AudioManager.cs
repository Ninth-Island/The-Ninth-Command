
using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine.Audio;
using UnityEngine;
using Random = UnityEngine.Random;


public class AudioManager : MonoBehaviour{


    [SerializeField] public List<Sound> sounds;
    [SerializeField] private GameObject newObjectSource;

    public AudioSource source;


    private void Awake(){
        source = GetComponent<AudioSource>();
    }


    [Client]
    public void PlayRepeating(int index, float time){ // for repeated noises that are attempted to be played every frame such as firing
        Sound sound = sounds[index];
        
        if (source.time >= sound.waitTillNext || !source.isPlaying){
            source.clip = sound.clipsList[Random.Range(0, sound.clipsList.Length)];

            source.Play();
            source.time = time;
        }
    }
    
    [Client]
    public void PlaySound(int index, bool allowInterrupt){
        Sound sound = sounds[index];

        if (allowInterrupt){
            source.Stop();
            source.clip = sound.clipsList[Random.Range(0, sound.clipsList.Length)];
            source.Play();
        }
        else{
            source.PlayOneShot(sound.clipsList[Random.Range(0, sound.clipsList.Length)], sound.volume);
        }
        
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.loop = sound.loop;
        source.priority = sound.priority;
        source.spatialBlend = sound.spacialBlend;

    }
    
    [Client]
    public void PlayConstant(int index, bool allowInterrupt, float time){ // for sounds that ramp up like charging that take time
        source.Stop();

        Sound sound = sounds[index];
        
        if (!source.isPlaying || allowInterrupt && source.clip != sound.clipsList[0]){


            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.loop = sound.loop;
            source.priority = sound.priority;
            source.spatialBlend = sound.spacialBlend;

            source.clip = sound.clipsList[Random.Range(0, sounds[index].clipsList.Length)];
            source.timeSamples = Mathf.RoundToInt(source.clip.samples * Mathf.Clamp(time, 0, 1)) -1;

            source.Play();
            
            
        }
    }

    
    [Client]
    public void PlayNewSource(int index){ // creates a new audio source to avoid interference with main one
        AudioSource newSource = Instantiate(newObjectSource, transform.position, Quaternion.identity).GetComponent<AudioSource>();
        newSource.clip = sounds[index].clipsList[Random.Range(0, sounds[index].clipsList.Length)];
        Sound sound = sounds[index];
        
        newSource.volume = sound.volume;
        newSource.pitch = sound.pitch;
        newSource.loop = sound.loop;
        newSource.priority = sound.priority;
        newSource.spatialBlend = sound.spacialBlend;
        newSource.Play();
        Debug.Log(newSource.volume);

        Destroy(newSource.gameObject, newSource.clip.length);
    }
    

}
