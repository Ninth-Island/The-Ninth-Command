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

    [SerializeField] protected bool sticky;
    [SerializeField] private float lifetime = 10f;
    
    private Collider2D _collider;
    public int damage;
    public float initialAngle;
    private bool _piercing;

    public Player firer;

    

    protected void Awake(){
        _collider = GetComponent<Collider2D>();
        Start();

        Destroy(gameObject, lifetime);
    }


    protected virtual void OnCollisionEnter2D(Collision2D other){
        if (sticky){
            transform.parent = other.gameObject.transform;
            body.velocity = new Vector2(0, 0);
            body.simulated = false;
        }
        _collider.enabled = false;
        body.constraints = RigidbodyConstraints2D.FreezeAll;
        gameObject.SetActive(false);
        if (other.rigidbody && other.rigidbody.sharedMaterial){
            if (other.rigidbody.sharedMaterial.name == "Metal"){
                AudioManager.PlayNewSource(0);
            }
            else if (other.rigidbody.sharedMaterial.name == "Snow" || other.rigidbody.sharedMaterial.name == "Rock" ||
                     other.rigidbody.sharedMaterial.name == "Grass"){
                AudioManager.PlayNewSource(1);
            }
        }

    }

    
    public virtual void SetValues(Player setFirer, int setDamage, float speed, float angle, bool piercing, int firedLayer, string setName){
        firer = setFirer;
        
        initialAngle = angle;
        damage = setDamage;
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
