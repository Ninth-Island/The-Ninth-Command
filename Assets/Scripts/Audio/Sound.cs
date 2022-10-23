using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Sound{
    public string name;
    
    public AudioClip[] clipsList;

    [Range(0f, 1f)] public float volume;
    [Range(0.1f, 3f)] public float pitch;


    [Range(0f, 1f)] public float spacialBlend;
    [Range(0, 256)] public int priority;
    public float waitTillNext;

    public bool loop;

    public Sound(string name, AudioClip[] clipsList, float volume, float pitch, float spacialBlend, int priority, float waitTillNext, bool loop){
        this.name = name;
        this.clipsList = clipsList;
        this.volume = volume;
        this.pitch = pitch;
        this.spacialBlend = spacialBlend;
        this.priority = priority;
        this.waitTillNext = waitTillNext;
        this.loop = loop;
    }
    
}
