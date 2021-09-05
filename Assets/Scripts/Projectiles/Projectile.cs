using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class Projectile : MonoBehaviour{
    
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
    
    protected Rigidbody2D Body;
    private Collider2D _collider;
    private int _damage;
    private bool _piercing;

    private bool _live = true;
    
    // Start is called before the first frame update
    

    // Update is called once per frame
    void Update(){
        
    }

    public void Awake(){
        Body = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        Destroy(gameObject, lifetime);
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
    public void SetValues(int damage, float speed, float angle, bool piercing, int firedLayer, string name){
        _damage = damage;
        gameObject.name = name + " " + gameObject;
        SetLayer(firedLayer);
        Body.velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed;
        transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        _piercing = piercing;
    }
    
    /*
     * ================================================================================================================
      *                                        Other
     * ================================================================================================================
     */


    private void SetLayer(int firedLayer){
        if (firedLayer == LayerMask.NameToLayer("Team 1")){
            gameObject.layer = LayerMask.NameToLayer("Team 1 Projectile");
        }
        if (firedLayer == LayerMask.NameToLayer("Team 2")){
            gameObject.layer = LayerMask.NameToLayer("Team 2 Projectile");
        }
        if (firedLayer == LayerMask.NameToLayer("Team 3")){
            gameObject.layer = LayerMask.NameToLayer("Team 3 Projectile");
        }
        if (firedLayer == LayerMask.NameToLayer("Team 4")){
            gameObject.layer = LayerMask.NameToLayer("Team 4 Projectile");
        }
    }
    
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
