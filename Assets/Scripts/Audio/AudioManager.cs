
using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine.Audio;
using UnityEngine;
using Random = UnityEngine.Random;


public class AudioManager : MonoBehaviour{


    [SerializeField] public List<Sound> sounds;
    [SerializeField] private GameObject newObjectSource;

    public AudioSource source;

    public bool isPlayingCharging;

    private void Awake(){
        source = GetComponent<AudioSource>();
    }


    // can play any noise and not interfere
    [Client]
    public void PlaySound(int index){
        Sound sound = sounds[index];
        SetSourceProperties(sound);
        source.PlayOneShot(sound.clipsList[Random.Range(0, sound.clipsList.Length)], sound.volume);
    }

    [Client]
    public IEnumerator PlaySoundDelayed(int index, float wait){
        yield return new WaitForSeconds(wait);
        PlaySound(index);
        
    }
    
    // for repeated noises that are attempted to be played every frame such as dry firing
    [Client]
    public void PlayRepeating(int index){ 
        Sound sound = sounds[index];
        
        if (source.time >= sound.waitTillNext || !source.isPlaying){
            source.clip = sound.clipsList[Random.Range(0, sound.clipsList.Length)];
            SetSourceProperties(sound);
            source.time = 0;
            source.Play();
        }
    }
    
    [Client]
    public void PlayChargingNoise(int index, float time){ // for sounds that ramp up like charging that take time spartan laser, shields, etc
        Sound sound = sounds[index];
        if (!isPlayingCharging || !source.isPlaying){ // if anything on source
            isPlayingCharging = true;
            source.Stop();
            SetSourceProperties(sound);
            source.clip = sound.clipsList[Random.Range(0, sounds[index].clipsList.Length)];
            source.timeSamples = (int)(source.clip.samples * Mathf.Clamp(time, 0, 0.99f));
            source.Play();
        }
    }

    [Client]
    public void PlayLooping(int index){
        Sound sound = sounds[index];
        SetSourceProperties(sound);
        if (!source.isPlaying || source.clip != sound.clipsList[0]){
            SetSourceProperties(sound);
            source.clip = sound.clipsList[Random.Range(0, sounds[index].clipsList.Length)];
            source.Play();
        }
    }

    
    public void PlayNewSource(int index){ // creates a new audio source to avoid interference with main one
        AudioSource newSource = Instantiate(newObjectSource, transform.position, Quaternion.identity).GetComponent<AudioSource>();
        newSource.clip = sounds[index].clipsList[Random.Range(0, sounds[index].clipsList.Length)];
        Sound sound = sounds[index];
        
        newSource.volume = sound.volume;
        newSource.pitch = sound.pitch;
        newSource.priority = sound.priority;
        newSource.spatialBlend = sound.spacialBlend;
        newSource.Play();

        Destroy(newSource.gameObject, newSource.clip.length);
    }

    [Client]
    private void SetSourceProperties(Sound sound){
        source.volume = sound.volume;
        source.pitch = sound.pitch;
        source.priority = sound.priority;
        source.spatialBlend = sound.spacialBlend;
    }

}
