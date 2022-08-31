using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Weapon : MonoBehaviour{



    [Header("Weapon")] 
    [SerializeField] private WeaponPickup weaponPickup;
    [SerializeField] protected Character wielder;
    [SerializeField] public Vector2 offset = new Vector2(1.69f, -0.42f);

    public SpriteRenderer spriteRenderer;
    public AudioManager AudioManager;
    

    
    public WeaponPickup GetWeaponPickup(){
        return weaponPickup;
    }

    public void SetWielder(Character setWielder){
        wielder = setWielder;
    }



    #region Start Update

    protected virtual void Start(){ 
    }
    
    protected virtual void Update(){
        
    }
    
    protected virtual void FixedUpdate(){
        
    }

    #endregion
    
    
}
