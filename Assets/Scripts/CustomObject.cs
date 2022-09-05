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
    public AudioManager audioManager;

    public Transform parent;
    public Vector2 localPos;
    


    public override void OnStartClient(){
        body = GetComponent<Rigidbody2D>();
    }

    [ClientCallback]
    protected virtual void FixedUpdate(){
        if (hasAuthority){
            CmdServerPositionUpdateHasParent();
        }
    }

    [Command]
    private void CmdServerPositionUpdateHasParent(){
        if (parent){
            Vector3 offset = new Vector3();
            transform.position = parent.transform.position + offset;
            transform.rotation = parent.transform.rotation;
            transform.localScale = parent.transform.lossyScale;
        }
    }

    [ClientCallback]
    protected virtual void Update(){
        
    }
}

