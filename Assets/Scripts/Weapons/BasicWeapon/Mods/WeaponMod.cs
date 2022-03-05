using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMod : CustomObject{

    [SerializeField] protected bool forcedOn = false;
    
    protected BasicWeapon WeaponAttachedTo;
    
    
    protected virtual void Start(){
        WeaponAttachedTo = GetComponent<BasicWeapon>();
    }

    protected virtual void Update(){
        
    }

    
}
