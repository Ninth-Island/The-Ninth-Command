using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class MeleeHitBox : MonoBehaviour
{
    private void Start(){
        gameObject.SetActive(false);
    }

    public void SetLayer(int layer){
        gameObject.layer = layer;
    }

    private void OnTriggerStay2D(Collider2D other){
        Debug.Log(other.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other){
        OnTriggerStay2D(other);
    }
}
