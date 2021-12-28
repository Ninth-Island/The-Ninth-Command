using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Vector2 = UnityEngine.Vector2;

public class NPC : Character{

    
    [Header("AI")]
    [SerializeField] float sightRange;
    [SerializeField] private float aiType = 0;
    // 0 = guard
    // 1 = attacker
    [SerializeField] private int idleType = 0;
    // 0 == stand still
    // 1 = back and forth
    [SerializeField] private Vector2 pointOfInterest = new Vector2(0.1f, 0.1f);
    // either a guard point or a capture zone

    [SerializeField] private float optimalFiringDistance;
    [SerializeField] private float randomRange;
    private GameObject _target;


    private BoxCollider2D wallDetector;
    private BoxCollider2D groundDetector;

    /*
     ================================================================================================================
                                        Target Finding
     ================================================================================================================
     */
    
    private void FindNearestTarget(){
        _target = null;
        int mask = LayerMask.GetMask("Team 1", "Team 2", "Team 3", "Team 4");
        mask = mask & ~(1 << gameObject.layer); // some bit shifting magic that excludes the layer of the object doing the scan

        RaycastHit2D[] potentialTargets = Physics2D.CircleCastAll(transform.position, sightRange, new Vector2(0, 0), 0, mask);
        foreach (RaycastHit2D potentialTarget in potentialTargets){
            LineOfSightCheck(potentialTarget, mask);
        }
    }

    private void LineOfSightCheck(RaycastHit2D potentialTarget, int mask){
        mask = mask | (1 << LayerMask.NameToLayer("Ground"));
        RaycastHit2D losc = Physics2D.Linecast(transform.position, potentialTarget.transform.position, mask);  // losc is short for Line Of Sight Check
        if (losc.collider.gameObject.layer != LayerMask.NameToLayer("Ground")){
            _target = losc.collider.gameObject;
            if (_target.transform.parent){
                _target = _target.transform.parent.gameObject;
            }
        }
    }


    private void SortAiBehavior(){
        
        if (aiType == 0f){ // if you're a guard...
            if (idleType == 1){
                Patrol();
            }
        }

        if (aiType == 1f){ // if you're an attacker...
            if (_target){
                MoveToPoint(_target.transform.position);
            }
            else if (pointOfInterest != new Vector2(0.1f, 0.1f)){ // there's no target and there IS an attack point
                MoveToPoint(pointOfInterest);
            }
            else{ // there's no target and there's no capture point
                if (idleType == 1){
                    Patrol();
                }
            }
        }
    }

    private void Patrol(){
        if (!InputsFrozen && !Knocked && !_target){
            Body.velocity = new Vector2(moveSpeed * transform.localScale.x * 0.3f, Body.velocity.y);
        }
    }

    private void MoveToPoint(Vector2 destination){
        
        float x = transform.position.x;
        float y = transform.position.y;
        int direction = 0;
        
        
        if (x + optimalFiringDistance < destination.x){
            direction = 1;
        }
        else if (x - optimalFiringDistance > destination.x){
            direction = -1;
        }
        
        Body.velocity = new Vector2(moveSpeed * direction, Body.velocity.y);
    }


    protected override void OnCollisionEnter2D(Collision2D other){
       
    }


    protected override void Start(){
        base.Start();

        wallDetector = transform.GetChild(2).GetComponent<BoxCollider2D>();
        groundDetector = transform.GetChild(3).GetComponent<BoxCollider2D>();

        optimalFiringDistance += Random.Range(-randomRange, randomRange);
        
        
        InvokeRepeating(nameof(FindNearestTarget), 0, 1f);
    }

    protected override void Update(){
        base.Update();
        
    }

    protected override void FixedUpdate(){
        base.FixedUpdate();
        SortAiBehavior();
    }
    
}
    
