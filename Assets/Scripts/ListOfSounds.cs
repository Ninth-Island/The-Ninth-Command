using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ListOfSounds{
    public string name;
    
    public AudioClip[] soundsList;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;    
    [Range(0f, 1f)] public float spacialBlend;
    [Range(0, 256)] public int priority = 128;
    public float waitTillNext = 0f;

    public bool loop;

    
    
    [HideInInspector] public AudioSource source;
}
