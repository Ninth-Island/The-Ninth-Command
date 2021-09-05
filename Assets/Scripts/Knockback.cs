using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Mathematics;
using UnityEngine;

public class Knockback : MonoBehaviour{
    
    /*
 * ================================================================================================================
 *                                               Knockback
  *
  *  Standalone for dealing knockback
  *
  * 
 * ================================================================================================================
 */

    [SerializeField] private float maxDamage;
    [SerializeField] private float minDamage;
    
    public float yScale = 4;
    public float xScale = 1;
    // Start is called before the first frame update
    void Start(){
        Destroy(gameObject, 0.6f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other){
        Vector2 pos = transform.position;
        Vector2 otherPos = other.transform.position;
        float xDist = otherPos.x - pos.x;
        float yDist = otherPos.y - pos.y;
        other.attachedRigidbody.velocity = new Vector2((Math.Sign(xDist) * 10 - xDist) * xScale, (Math.Sign(yDist) * 10 - yDist)) * yScale;
        // do some damage logic here
    }

}
