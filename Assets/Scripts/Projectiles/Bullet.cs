using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    /*
* ================================================================================================================
*                                               Bullet
*
*  A bullet that may bounce off walls. Effective at dealing damage once shields are down
* 
* ================================================================================================================
*/

    [SerializeField] private GameObject deadBullet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void OnCollisionEnter2D(Collision2D other){
        base.OnCollisionEnter2D(other);
        GameObject DB = Instantiate(deadBullet, transform.position, transform.rotation);
        DB.GetComponent<Rigidbody2D>().velocity = gameObject.GetComponent<Rigidbody2D>().velocity;
        Destroy(DB, 2f);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
