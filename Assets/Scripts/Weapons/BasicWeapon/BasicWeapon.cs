using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

public class BasicWeapon : Weapon{

    [SerializeField][Tooltip("Player Only")] public int armType = 0;
    [SerializeField][Tooltip("Player Only")] public int cursorType = 0;
    [SerializeField] private bool allowInterrupt = false;
    public Transform firingPoint;

    public bool activelyWielded = false;
    

    public virtual void AttemptFire(float angle){
        
    }
    
    protected virtual void HandleMagazineDecrement(){
        AudioManager.PlaySound(0, allowInterrupt);
    }
    

    public virtual void RefreshText(){
        
    }

    public virtual void Reload(){
        AudioManager.PlaySound(1, false);
    }

    public virtual void Ready(){
        AudioManager.PlaySound(2, allowInterrupt);
        activelyWielded = true;
        RefreshText();
    }
    


    public void SetSpriteRenderer(bool setEnabled){
        spriteRenderer.enabled = setEnabled;
    }

    protected override void Start(){
        base.Start();

        transform.localRotation = new Quaternion(0, 0, 0, 0);
        transform.localScale = new Vector3(Math.Abs(transform.localScale.x), Math.Abs(transform.localScale.y));
        activelyWielded = true;
        RefreshText();
        
    }

    protected override void Update(){
        base.Update();
    }
    
    protected override void FixedUpdate(){
        
    }

    private void OnDisable(){
        //AudioManager.source.Stop();
    }
    
}
