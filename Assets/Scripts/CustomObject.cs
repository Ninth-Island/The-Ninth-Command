using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CustomObject : NetworkBehaviour{



    public Rigidbody2D body;
    public Collider2D Collider;
    public SpriteRenderer spriteRenderer;
    protected AudioManager AudioManager;

    protected Transform Parent;
    protected Vector2 LocalPos;
    
    
    public override void OnStartClient(){
        body = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        //sprite renderer follows a different path each time
        AudioManager = GetComponent<AudioManager>();
    }
    
    [ClientCallback]
    protected virtual void Update(){
        
    }

    [ClientCallback]
    protected virtual void FixedUpdate(){
        if (hasAuthority){
            CmdServerPositionUpdateHasParent();
        }
    }
    
    [Command]
    private void CmdServerPositionUpdateHasParent(){
        if (Parent){
            Vector3 offset = new Vector3(); // make this smth good!
            transform.position = Parent.transform.position + offset;
            transform.rotation = Parent.transform.rotation;
            transform.localScale = Parent.transform.lossyScale;
        }
    }

}

