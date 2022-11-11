using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using Mirror;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class BasicWeapon : Weapon{

    [SerializeField][Tooltip("Player Only")] public int armType = 0;
    [SerializeField][Tooltip("Player Only")] public int cursorType = 0;
    
    public Transform firingPoint;
    public bool activelyWielded = false;
    public WeaponMod weaponMod;


    
    [Header("Scope")]


    public int zoomIncrements;
    [FormerlySerializedAs("zoomPerIncrement")] [SerializeField] private float totalZoom;

    [SerializeField] private Transform lookAt;
    [HideInInspector] public float zoomPerIncrement;
    private CursorControl _cursorControl;

    [SerializeField] private int zoomInStartIndex;
    private int _currentIncrement;

    private Camera _mainCam;
    
    
    public override void OnStartClient(){
        base.OnStartClient();
        zoomPerIncrement = totalZoom / zoomIncrements;
        _mainCam = Camera.main;
    }

    public virtual void Ready(){
        activelyWielded = true;
        audioManager.PlaySound(2);
    }

    protected override void Pickup(Player player, int[] path){
        base.Pickup(player, path);
        Ready();
        _cursorControl = player.transform.GetChild(3).GetComponent<CursorControl>();
    }

    [Command]
    public void CmdAttemptFire(float angle){
        HandleFiring(angle);    
    }
    
    public virtual void HandleFiring(float angle){
        
    }

    
    protected virtual void HandleMagazineDecrement(){
        audioManager.PlaySound(0);
    }


    public virtual void StopReloading(){
        StopAllCoroutines();
        ResetZoom();
    }

    protected virtual void RefreshText(){
        
    }
    
     
    protected override void ClientUpdate(){
        base.ClientUpdate();
        if (activelyWielded){
            RefreshText();

            if (_currentIncrement > 0){
                lookAt.position = _mainCam.ScreenToWorldPoint(Input.mousePosition);
                float maxZoom = zoomPerIncrement * _currentIncrement;
                Vector2 position = wielder.transform.position;
                lookAt.position = new Vector3(Mathf.Clamp(lookAt.position.x, position.x - maxZoom, position.x + maxZoom), Mathf.Clamp(lookAt.position.y, position.y - maxZoom, position.y + maxZoom));
            }
        }
    }


    public virtual void Reload(){
        audioManager.PlaySound(1);
        ResetZoom();
    }


    public override void SwapTo(Player player, Equipment oldEquipment, int[] path){
        base.SwapTo(player, oldEquipment, path);
        
        spriteRenderer.sortingLayerID = player.spriteRenderer.sortingLayerID;
        spriteRenderer.sortingOrder = 4;

        player.primaryWeapon = this;

    }


    [Server]
    public override IEnumerator ServerInitializeEquipment(bool isThePrimaryWeapon, Player player, int[] path){
        wielder = player;
        netIdentity.AssignClientAuthority(player.connectionToClient);
        ClientSetWielder(player);

        yield return new WaitUntil(() => player.characterClientReady);
        CancelPickup(player, path);
        ClientInitializeEquipment(isThePrimaryWeapon, player, path);
    }

    [ClientRpc]
    protected override void ClientInitializeEquipment(bool isThePrimaryWeapon, Player player, int[] path){
        base.ClientInitializeEquipment(isThePrimaryWeapon, player, path);
        if (!isThePrimaryWeapon){
            activelyWielded = false;
            spriteRenderer.enabled = false;
        }
        
    }


    public override void Drop(){
        base.Drop();
        wielder = null;
        activelyWielded = false;
        spriteRenderer.enabled = true;
        ResetZoom();
    }

    public virtual void PutAway(){
        ResetZoom();
    }

    public void Zoom(){
        if (zoomIncrements > 0){
            _currentIncrement++;

            if (_currentIncrement > zoomIncrements){
                ResetZoom();
            }
            else{
                _cursorControl.CameraFollow(lookAt);
                audioManager.PlaySound(zoomInStartIndex);
            }
        }
    }

    public void ResetZoom(){
        if (zoomIncrements > 0 && _currentIncrement > 0){
            _currentIncrement = 0;
            _cursorControl.ResetCamera();
            audioManager.PlaySound(zoomInStartIndex + 1);
        }
    }


}

