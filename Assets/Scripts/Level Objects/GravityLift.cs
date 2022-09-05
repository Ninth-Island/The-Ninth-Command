using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLift : MonoBehaviour{

    [SerializeField] private float boost;
    

    private void OnTriggerEnter2D(Collider2D other){
        float angle = (Mathf.PI / 2) + transform.parent.transform.rotation.z;
        Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        other.attachedRigidbody.velocity = velocity.normalized * boost;
    }
}
