using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomObject : MonoBehaviour{

    [SerializeField] public readonly float TerminalVelocity = -100f;

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
