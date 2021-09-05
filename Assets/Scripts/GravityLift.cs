using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityLift : MonoBehaviour{

    [SerializeField] private float boost;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other){
        float angle = 1.57079632679f + transform.parent.transform.rotation.z;
        Vector2 velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        other.attachedRigidbody.velocity = velocity.normalized * boost;
        Character characterScript = other.GetComponent<Character>();
        
        characterScript?.SetInputFrozen(true, (-Physics2D.gravity.y) / boost);
       
    }
}
