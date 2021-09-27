using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallDetector : MonoBehaviour{
    
    [SerializeField] private bool wallDetector;

    private Vector2 ls;
    private void Start(){ 
        ls = transform.parent.transform.localScale;
    }

    private void FixedUpdate(){
        if (!wallDetector){
            RaycastHit2D groundCast = Physics2D.Raycast(transform.position, Vector2.down, 4, LayerMask.GetMask("Ground"));
            Debug.DrawLine(transform.position, new Vector3(transform.position.x, transform.position.y - 4));
            if (!groundCast){
                transform.parent.transform.localScale = new Vector3(transform.parent.transform.localScale.x * -1, 1);            
            } 
        }
    }

    private void OnTriggerEnter2D(Collider2D other){
        if (wallDetector){
            transform.parent.transform.localScale = new Vector3(transform.parent.transform.localScale.x * -1, 1);
        }
    }
    
}