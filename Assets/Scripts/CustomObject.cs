using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CustomObject : NetworkBehaviour{

    [SerializeField] public readonly float TerminalVelocity = -100f;

    public Transform parent;
    public Vector3 localPos;
    public float localRot;
    public Vector3 localScale;
    
    public int spriteLayer;

    public Rigidbody2D body;

    protected virtual void Start(){
        body = GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate(){
        if (body && body.velocity.y < TerminalVelocity){
            body.velocity = new Vector2(body.velocity.x, TerminalVelocity);
        }
    }

    protected virtual void Update(){
    }
}
