using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Pathfinding;
using Vector2 = UnityEngine.Vector2;

public class NPC : Character{

    [SerializeField] float sightRange;

    private GameObject _target;

    private void FindNearestTarget(){

        _target = null;

        
        // Gets all characters within the sight radius
        RaycastHit2D[] results = Physics2D.CircleCastAll(transform.position, sightRange, new Vector2(0, 0), 0, LayerMask.GetMask("Team 1", "Team 2", "Team 3", "Team 4"));
        
        
        // for every character within radius....
        foreach (RaycastHit2D raycastHit in results){
            
            // check for null and to make sure it hasn't caught itself,
            if (raycastHit && raycastHit.collider.gameObject.layer != gameObject.layer){
                Vector2 potentialTarget = raycastHit.collider.transform.position;
                
                // and if that's true, then make sure the character is in line of sight (not being blocked by a wall)
                RaycastHit2D lineOfSightCheck = Physics2D.Raycast(transform.position, new Vector2(potentialTarget.x - transform.position.x, potentialTarget.y - transform.position.x), sightRange, LayerMask.GetMask("Team 1", "Team 2", "Team 3", "Team 4", "Ground"));
                if (!lineOfSightCheck.collider.CompareTag("Ground")){
                    Debug.Log(lineOfSightCheck.collider.gameObject);
                    // Check to see if there isn't already a target
                    if (_target is null){
                        _target = raycastHit.collider.gameObject;
                    }
                    else{
                        // Finally, if there is already a current target, check to see if this target is closer. If it is, this is the new target
                        if (Vector2.Distance(transform.position, raycastHit.collider.transform.position) < Vector2.Distance(transform.position, _target.transform.position)){
                            _target = raycastHit.collider.gameObject;
                        }
                    }
                }
            }
        }
    }


    protected override void OnCollisionEnter2D(Collision2D other){
       
    }

    protected override void Start(){
        base.Start();
        
        InvokeRepeating(nameof(FindNearestTarget), 0, 1f);
    }

    protected override void Update(){
        base.Update();
        if (_target){
            Debug.DrawLine(transform.position, _target.transform.position);
//            Debug.Log(_target);
        }
    }

    protected override void FixedUpdate(){
        base.FixedUpdate();
    }
    
}
    
