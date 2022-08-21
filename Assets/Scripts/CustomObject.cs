using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CustomObject : NetworkBehaviour{


    public Transform parent;
    public Vector3 localPos;
    public float localRot;
    public Vector3 localScale;
    
    public int spriteLayer;

    public Rigidbody2D body;


    public override void OnStartClient(){
        body = GetComponent<Rigidbody2D>();
    }

    [ClientCallback]
    protected virtual void FixedUpdate(){
    }

    [ClientCallback]
    protected virtual void Update(){
        if (parent && hasAuthority){
            transform.position = parent.position + localPos;
            transform.rotation = Quaternion.Euler(parent.transform.eulerAngles.x, parent.transform.eulerAngles.y, parent.transform.eulerAngles.z + localRot);
            transform.localScale = new Vector3(parent.transform.localScale.x * localScale.x, parent.transform.localScale.y, 1);
        }
    }
}
