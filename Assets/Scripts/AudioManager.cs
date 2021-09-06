using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;


public class AudioManager : MonoBehaviour{

    [Tooltip("For Many Sounds Representing same action such as walking")]
    public ListOfSounds[] soundsFromList;

    [Tooltip("For a single sound representing the same action such as voice lines")]
    public List<Sound> sounds;


    private void Awake(){
        foreach (Sound sound in sounds){
            
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip; 
            sound.source.volume = sound.volume; 
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.spatialBlend = sound.spacialBlend;
        }

        foreach (ListOfSounds listOfSounds in soundsFromList){
            listOfSounds.source = gameObject.AddComponent<AudioSource>();
            listOfSounds.source.volume = listOfSounds.volume; 
            listOfSounds.source.pitch = listOfSounds.pitch;
            listOfSounds.source.loop = listOfSounds.loop;
            listOfSounds.source.spatialBlend = listOfSounds.spacialBlend;
        }
    
    }


    public void PlayFromList(int listIndex){
        ListOfSounds list = soundsFromList[listIndex];
        if (list.random == true){
            AudioClip clip = list.soundsList[Random.Range(0, list.soundsList.Length)];
            list.source.clip = clip;
            list.source.Play();
        }
        else{
            
        }
    }

    public void PlayClip(int clipIndex){
        Sound clip = sounds[clipIndex];
        if (clip.source.isPlaying && clip.source.time >= clip.waitTillNext || !clip.source.isPlaying){
            clip.source.Play();
        }
    }
}
