using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Projectile : CustomObject{
    
    /*
 * ================================================================================================================
 *                                               Projectile
  *
  *  The super parent of all projectiles. Rockets, bullets, grenades, etc
     *
     * contains logic for dealing damage to characters, sticking to surfaces, and a way for Instantiators to set values
  *
  * 
 * ================================================================================================================
 */

    [SerializeField] private bool sticky;
    [SerializeField] private float lifetime = 10f;
    
    private Collider2D _collider;
    private int _damage;
    private bool _piercing;

    private Character _firer;

    [SerializeField] protected bool _live = true;
    

    protected void Awake(){
        _collider = GetComponent<Collider2D>();
        Start();

        if (_live){
            Destroy(gameObject, lifetime);
        }
        /*if (_live && isServer){
            StartCoroutine(ServerDestroy(gameObject, lifetime));
        }*/
    }
    
    

    protected virtual void OnCollisionEnter2D(Collision2D other){
        if (sticky){
            transform.parent = other.gameObject.transform;
            body.velocity = new Vector2(0, 0);
            body.simulated = false;
        }

        Character character = other.gameObject.GetComponent<Character>();
        if (character && _live){
            character.Hit(_damage);
        }
        
        body.mass = 1;
        _live = false;
        gameObject.layer = LayerMask.NameToLayer("Dead Projectiles");
        
    }

    
    public virtual void SetValues(Character firer, int damage, float speed, float angle, bool piercing, int firedLayer, string setName){
        _firer = firer;

        
        _damage = damage;
        name = setName + " " + gameObject;
        gameObject.layer = firedLayer - 4;
        _piercing = piercing;

        Awake();
        StartCoroutine(SetupProjectile(angle, speed));
    }

    
    private IEnumerator SetupProjectile(float angle, float speed){
        yield return new WaitForEndOfFrame();
        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        body.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized * speed;
    }


    /*
     * ================================================================================================================
      *                                        Other
     * ================================================================================================================
     */

    public Collider2D GetCollider(){
        return _collider;
    }


}
