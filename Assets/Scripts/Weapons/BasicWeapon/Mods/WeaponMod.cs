using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMod : CustomObject{

    [SerializeField] protected bool forcedOn = false;
    
    protected BasicWeapon WeaponAttachedTo;
    protected AudioManager AudioManager;

    
    protected virtual void Start(){
        WeaponAttachedTo = transform.parent.GetComponent<BasicWeapon>();
        AudioManager = GetComponent<AudioManager>();
        
    }

    protected virtual void Update(){
        
    }

    
}
