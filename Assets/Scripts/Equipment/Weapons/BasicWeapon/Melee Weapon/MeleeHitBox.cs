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
                    parent.audioManager.PlaySound(4);
                }

                if (hit){
                    parent.audioManager.PlaySound(3);
                    gameObject.SetActive(false);
                    Debug.Log("player");
                }
            }
        }

        else{
            if (hit){
                hit.Hit(parent.wielder, parent.damage, parent.transform.position, 0);
                gameObject.SetActive(false);
            }
        }
    }
    
}
