using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Bullet : Projectile{
    /*
* ================================================================================================================
*                                               Bullet
*
*  A bullet that may bounce off walls. Effective at dealing damage once shields are down
* 
* ================================================================================================================
*/

    [SerializeField] private GameObject deadBullet;


    public override void OnStartServer(){
        base.OnStartServer();
    }
    
    protected override void OnCollisionEnter2D(Collision2D other){
        base.OnCollisionEnter2D(other);
        Destroy(gameObject, 0.02f);
        GameObject db = Instantiate(deadBullet, transform.position, transform.rotation);
        db.GetComponent<Rigidbody2D>().velocity = gameObject.GetComponent<Rigidbody2D>().velocity / 3;
        Destroy(db, 2f);
    
    }

}
