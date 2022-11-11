using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class CustomObject : NetworkBehaviour{


    [Header("Custom Object")]
    public Rigidbody2D body;
    public Collider2D Collider;
    public SpriteRenderer spriteRenderer;
    public AudioManager audioManager;

    public Transform parent;
    public Vector2 localPos;

    private float _angle;


    protected virtual void Start(){
        body = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        audioManager = GetComponent<AudioManager>();
    
    }
    
    
    protected virtual void FixedUpdate(){
        if (isClient){
            ClientFixedUpdate();
        }
        if (isServer){
            ServerFixedUpdate();
        }
    }

    protected virtual void Update(){
        if (isClient && hasAuthority){
            ClientUpdate();
        }
        if (isServer){
            ServerUpdate();
        }
    }


    [Server]
    protected virtual void ServerUpdate(){
        
    }

    [Server]
    protected virtual void ServerFixedUpdate(){
    }

    [Client]
    protected virtual void ClientUpdate(){
        
    }
    
    [Client]
    protected virtual void ClientFixedUpdate(){
        if (parent){
            _angle = parent.rotation.eulerAngles.z * Mathf.Deg2Rad;
            StartCoroutine(ServerPositionUpdateHasParent());
        }
    }
    
    
    private IEnumerator ServerPositionUpdateHasParent(){

        yield return new WaitForFixedUpdate();
            

        float yMultiplier = 1;
        if (parent.localScale.x < 0){
            yMultiplier = -1;
        }
    
        Vector2 xOffsetPoint = new Vector2(Mathf.Cos(_angle), Mathf.Sin(_angle)) * localPos.x;
        Vector3 offset = new Vector3(
            localPos.y * Mathf.Cos(_angle - Mathf.PI / 2) + xOffsetPoint.x,
            localPos.y * yMultiplier * Mathf.Sin(_angle - Mathf.PI / 2) + xOffsetPoint.y);

        transform.position = parent.position + offset;
        transform.rotation = parent.rotation;
        transform.localScale = parent.lossyScale;
        
    }

    [Server]
    protected IEnumerator ServerDestroy(GameObject obj, float time){
        yield return new WaitForSeconds(time);
        NetworkServer.Destroy(obj);
    }
  

}