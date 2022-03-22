
using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using Random = UnityEngine.Random;


public class AudioManager : MonoBehaviour{


    [SerializeField] public List<Sound> sounds;

    public AudioSource source;


    private void Awake(){
        source = GetComponent<AudioSource>();
    }


    private void PlayRepeating(Sound sound, float time){
        if (source.time >= sound.waitTillNext || !source.isPlaying){
            source.clip = sound.clipsList[Random.Range(0, sound.clipsList.Length)];

            source.Play();

            source.time = time;



        }
    }

    public void PlaySound(int index, bool repeating, float time){
        Sound sound = sounds[index];

        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.loop = sound.loop;
        source.priority = sound.priority;
        source.spatialBlend = sound.spacialBlend;
        
        if (repeating){
            PlayRepeating(sound, time);
        }
        else{
            source.PlayOneShot(sound.clipsList[Random.Range(0, sound.clipsList.Length)]);
        }
    }

    public void PlayConstant(int index, bool allowInterrupt){

        Sound sound = sounds[index];
        
        if (!source.isPlaying || allowInterrupt && source.clip != sound.clipsList[0]){


            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.loop = sound.loop;
            source.priority = sound.priority;
            source.spatialBlend = sound.spacialBlend;

            source.clip = sound.clipsList[Random.Range(0, sounds[index].clipsList.Length)];
            source.Play();
        }
    }

    public void PlayAtPoint(int index){
        AudioSource newSource = gameObject.AddComponent<AudioSource>();
        newSource.clip = sounds[index].clipsList[Random.Range(0, sounds[index].clipsList.Length)];
        Sound sound = sounds[index];
        
        newSource.volume = sound.volume;
        newSource.pitch = sound.pitch;
        newSource.loop = sound.loop;
        newSource.priority = sound.priority;
        newSource.spatialBlend = sound.spacialBlend;
        newSource.Play();
        
        Destroy(newSource, 5f);
    }
    

}
