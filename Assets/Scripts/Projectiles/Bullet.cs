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
  

    protected override void OnCollisionEnter2D(Collision2D other){
        base.OnCollisionEnter2D(other);
        GameObject DB = Instantiate(deadBullet, transform.position, transform.rotation);
        DB.GetComponent<Rigidbody2D>().velocity = gameObject.GetComponent<Rigidbody2D>().velocity / 3;
        StartCoroutine(ServerDestroy(DB, 2));
        StartCoroutine(ServerDestroy(gameObject, 0));
    }

    public override IEnumerator SetValues(int damage, float speed, float angle, bool piercing, int firedLayer, string name){
        StartCoroutine(base.SetValues(damage, speed, angle, piercing, firedLayer, name));
        yield break;
    }
}
