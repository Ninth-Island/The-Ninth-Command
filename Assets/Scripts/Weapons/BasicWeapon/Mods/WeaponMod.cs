using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMod : CustomObject{

    [SerializeField] protected bool forcedOn = false;
    
    protected ProjectileWeapon WeaponAttachedTo;
    protected AudioManager AudioManager;

    
    protected override void Start(){
        WeaponAttachedTo = transform.parent.GetComponent<ProjectileWeapon>();
        AudioManager = GetComponent<AudioManager>();
        
    }

    protected override void Update(){
        
    }

    
}
