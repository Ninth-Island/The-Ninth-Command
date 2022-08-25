using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CustomObject : NetworkBehaviour{



    public Rigidbody2D body;
    public SpriteRenderer spriteRenderer;


    public override void OnStartClient(){
        body = GetComponent<Rigidbody2D>();
    }

    [ClientCallback]
    protected virtual void FixedUpdate(){
    }

    [ClientCallback]
    protected virtual void Update(){
        
    }
}

