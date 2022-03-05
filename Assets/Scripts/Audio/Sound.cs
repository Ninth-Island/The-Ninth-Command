using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Sound{
    public string name;
    
    [Tooltip("use only if playing a random sound")] 
    public AudioClip[] clipsList;

    [Range(0f, 1f)] public float volume = 1f;
    [Range(0.1f, 3f)] public float pitch = 1f;    
    [Range(0f, 1f)] public float spacialBlend;
    [Range(0, 256)] public int priority = 128;
    public float waitTillNext = 0f;

    public bool loop;

    
}
