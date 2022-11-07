using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Equipment : CustomObject
{
    
    
    [Header("Equipment")]
    [SerializeField] public Player wielder;

    [SerializeField] protected NetworkTransform networkTransform;

    public virtual void SwapTo(Player player, Equipment oldEquipment, int[] path){
        oldEquipment.Drop();
        Pickup(player, path);

    }
    
    public void CancelPickup(Player player, int[] path){
        Pickup(player, path);
        
        spriteRenderer.sortingLayerID = player.spriteRenderer.sortingLayerID;
        spriteRenderer.sortingOrder = 4;

    }

    protected virtual void Pickup(Player player, int[] path){
        parent = player.transform;
        networkTransform.enabled = false;
        
        for (int i = 0; i < path.Length; i++){
            parent = parent.GetChild(path[i]);
        }
        wielder = player;
        body.simulated = false;
    }



    public virtual void Drop(){
        body.simulated = true;
        body.bodyType = RigidbodyType2D.Dynamic;
        gameObject.layer = LayerMask.NameToLayer("Objects");
        parent = null;
        networkTransform.enabled = true;
        

        spriteRenderer.sortingLayerID = SortingLayer.NameToID("Objects");
        spriteRenderer.sortingOrder = 0;
    }


    
}
