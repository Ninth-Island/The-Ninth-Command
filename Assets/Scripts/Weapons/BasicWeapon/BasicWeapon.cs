using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.Mathematics;
using UnityEngine;

public class BasicWeapon : Weapon{

    [SerializeField] public Vector2 offset = new Vector2(1.69f, -0.42f);
    [SerializeField][Tooltip("Player Only")] public int armType = 0;
    [SerializeField][Tooltip("Player Only")] public int cursorType = 0;
    [SerializeField] private bool allowInterrupt = false;
    protected CursorControl CursorControl;
    public Transform firingPoint;

    public bool activelyWielded = false;

    protected Coroutine Coroutine;
    
    public bool looping;


    
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


    public override void PickUp(Character character){
        base.PickUp(character);

        transform.localRotation = new Quaternion(0, 0, 0, 0);
        transform.localScale = new Vector3(Math.Abs(transform.localScale.x), Math.Abs(transform.localScale.y));
        activelyWielded = true;
        RefreshText();
    }
    
    public override void Drop(){
        base.Drop();
        wielder = null;
        activelyWielded = false;
    }


    public void SetSpriteRenderer(bool setEnabled){
        spriteRenderer.enabled = setEnabled;
    }

    public override void OnStartClient(){
        base.OnStartClient();
        
        CursorControl = FindObjectOfType<CursorControl>();
        foreach (Player player in FindObjectsOfType<Player>()){
            player.AddWeapon(new KeyValuePair<GameObject, KeyValuePair<BasicWeapon, Rigidbody2D>>(gameObject, new KeyValuePair<BasicWeapon, Rigidbody2D>(this, body)));
        }
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
