using System;
using System.Collections;
using System.Collections.Generic;
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

    protected int _firedLayer;

    [SerializeField] protected bool _live = true;
    
    
    

    // Update is called once per frame
    protected override void Update(){
        
    }

    protected override void Start(){
        base.Start();
    }

    protected void Awake(){
        _collider = GetComponent<Collider2D>();
        Start();
        if (_live){
            Destroy(gameObject, lifetime);
        }
    }
    /*
     * ================================================================================================================
      *                                        Collision Logic
     * ================================================================================================================
     */
    

    protected virtual void OnCollisionEnter2D(Collision2D other){
        if (sticky){
            transform.parent = other.gameObject.transform;
            Body.velocity = new Vector2(0, 0);
            Body.simulated = false;
        }

        Character character = other.gameObject.GetComponent<Character>();
        if (character && _live){
            character.Hit(this);
        }
        
        Body.mass = 1;
        _live = false;
        gameObject.layer = LayerMask.NameToLayer("Dead Projectiles");
    }


    /*
     * ================================================================================================================
      *                                        Set Values for Instantiators
     * ================================================================================================================
     */
    public virtual void SetValues(int damage, float speed, float angle, bool piercing, int firedLayer, string name){
        _damage = damage;
        gameObject.name = name + " " + gameObject;
        gameObject.layer = firedLayer - 4;
        _firedLayer = firedLayer;
        //Body.velocity += new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
        Awake();
        Body.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        _piercing = piercing;
    }
    
    /*
     * ================================================================================================================
      *                                        Other
     * ================================================================================================================
     */
    
    
    public Collider2D GetCollider(){
        return _collider;
    }

    public int GetDamage(){
        return _damage;
    }

    public bool GetLive(){
        return _live;
    }

}
