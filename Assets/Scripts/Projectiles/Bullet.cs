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
    private Level _level;


    public override void OnStartServer(){
        base.OnStartServer();
        _level = FindObjectOfType<Level>();
    }

    protected override void OnCollisionEnter2D(Collision2D other){
        base.OnCollisionEnter2D(other);
        Destroy(gameObject);
        GameObject db = Instantiate(deadBullet, transform.position, transform.rotation);
        db.GetComponent<Rigidbody2D>().velocity = gameObject.GetComponent<Rigidbody2D>().velocity / 3;
        Destroy(db, 2f);
        
    }
    
    public override void SetValues(int damage, float speed, float angle, bool piercing, int firedLayer, string name){
        base.SetValues(damage, speed, angle, piercing, firedLayer, name);
    }
}
