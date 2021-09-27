using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;
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
    private GameObject _target;


    private BoxCollider2D wallDetector;
    private BoxCollider2D groundDetector;



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
                // go attack target!
            }
            else if (pointOfInterest != new Vector2(0.1f, 0.1f)){ // there's no target and there IS an attack point
                // go take the capture point!
            }
            else{ // there's no target and there's no capture point
                Patrol();
            }
        }
    }

    private void Patrol(){
        if (!InputsFrozen && !Knocked){
            Body.velocity = new Vector2(moveSpeed * transform.localScale.x, Body.velocity.y);
        }
    }

    private void Move(){
        
    }


    protected override void OnCollisionEnter2D(Collision2D other){
       
    }


    protected override void Start(){
        base.Start();

        wallDetector = transform.GetChild(2).GetComponent<BoxCollider2D>();
        groundDetector = transform.GetChild(3).GetComponent<BoxCollider2D>();
        
        
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
    
