using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMod : MonoBehaviour{

    [SerializeField] protected bool forcedOn = false;
    
    protected ProjectileWeapon WeaponAttachedTo;
    protected AudioManager AudioManager;

    
    protected virtual void Start(){
        //WeaponAttachedTo = transform.parent.GetComponent<ProjectileWeapon>();
        AudioManager = GetComponent<AudioManager>();
        
    }

    protected /*override*/virtual void Update(){
        
    }

    
}
