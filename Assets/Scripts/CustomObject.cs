using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomObject : MonoBehaviour{

    [SerializeField] public readonly float TerminalVelocity = -100f;

    public int spriteLayer;

    protected Rigidbody2D Body;

    protected virtual void Start(){
        Body = GetComponent<Rigidbody2D>();
    }

    protected virtual void FixedUpdate(){
        if (Body && Body.velocity.y < TerminalVelocity){
            Body.velocity = new Vector2(Body.velocity.x, TerminalVelocity);
        }
    }

    protected virtual void Update(){
        
    }
}
