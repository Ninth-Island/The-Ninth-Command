using System;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Random = UnityEngine.Random;


public class AudioManager : MonoBehaviour{


    [SerializeField] private List<Sound> sounds;

    private AudioSource _primarySource;

    
    private void Awake(){
        _primarySource = GetComponent<AudioSource>();
    }


    public void PlaySound(int index){
        Play(index, _primarySource);
    }

    public void PlaySoundFromSource(int index, AudioSource source){
        Play(index, source);
    }
    private void Play(int index, AudioSource source){

        Sound sound = sounds[index];
        
        if (source.time >= sound.waitTillNext || !source.isPlaying){
                        
            source.volume = sound.volume;
            source.pitch = sound.pitch;
            source.loop = sound.loop;
            source.priority = sound.priority;
            source.spatialBlend = sound.spacialBlend;

            if (sound.clip != null){
                source.clip = sound.clip;
            }
            else{
                source.clip = sound.clipsList[Random.Range(0, sound.clipsList.Length)];
            }
            
            source.Play();
        }

    }

    
    
}
