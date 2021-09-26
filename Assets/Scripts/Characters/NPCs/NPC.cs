using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Pathfinding;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class NPC : Character{

    [SerializeField] float sightRange;

    private GameObject _target;



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
            Debug.Log(_target);
        }
    }

    protected override void FixedUpdate(){
        base.FixedUpdate();
    }
    
}
    
