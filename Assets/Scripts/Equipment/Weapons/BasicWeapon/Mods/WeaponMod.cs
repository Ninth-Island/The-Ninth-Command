using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMod : CustomObject{

    public BasicWeapon WeaponAttachedTo;
    protected bool Active;

    protected virtual void ModActiveFixedUpdate(){
        
    }
    
    public virtual void WeaponModFixedUpdate(){
        if (Active){
            ModActiveFixedUpdate();
        }
    }

    public virtual void WeaponModInstant(){
        
    }

    public virtual void WeaponModRelease(){
        
    }


    
}
