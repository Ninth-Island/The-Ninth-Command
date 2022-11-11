using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class MeleeHitBox : MonoBehaviour{
    
    [SerializeField] private MeleeWeapon parent;

    private void OnCollisionEnter2D(Collision2D other){
        Player hit = other.gameObject.GetComponent<Player>();
        if (!parent.isServer){
            if (other.rigidbody && other.rigidbody.sharedMaterial){
                if (other.rigidbody.sharedMaterial.name == "Metal"){
                    parent.audioManager.PlayNewSource(3, -1);
                }

                if (hit){
                    parent.audioManager.PlayNewSource(4, -1);
                }
            }
        }

        else{
            if (hit){
                hit.Hit(parent.wielder, parent.damage, parent.transform.position, 0);
            }
        }
    }
    
}
