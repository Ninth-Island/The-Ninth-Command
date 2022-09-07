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

    public Transform parent;
    public Vector2 localPos;


    public override void OnStartClient(){
        body = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        //sprite renderer follows a different path each time
        AudioManager = GetComponent<AudioManager>();
    }

    [ClientCallback]
    protected virtual void Update(){
    }

    [ServerCallback]
    protected virtual void FixedUpdate(){
        StartCoroutine(ServerPositionUpdateHasParent());
    }

    [Server]
    private IEnumerator ServerPositionUpdateHasParent(){
        if (parent){

            yield return new WaitForFixedUpdate();
            if (parent){
            
                float angle = parent.rotation.eulerAngles.z * Mathf.Deg2Rad;

                float yMultiplier = 1;
                if (parent.localScale.x < 0){
                    yMultiplier = -1;
                }
            
                Vector2 xOffsetPoint = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * localPos.x;
                Vector3 offset = new Vector3(
                    localPos.y * Mathf.Cos(angle - Mathf.PI / 2) + xOffsetPoint.x,
                    localPos.y * yMultiplier * Mathf.Sin(angle - Mathf.PI / 2) + xOffsetPoint.y);

                transform.position = parent.position + offset;
                transform.rotation = parent.rotation;
                transform.localScale = parent.lossyScale;
            }
        }
    }
  

}