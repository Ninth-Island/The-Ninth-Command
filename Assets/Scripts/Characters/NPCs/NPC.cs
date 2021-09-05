using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class NPC : Character{
    
    
    [SerializeField] private float aiType;
    [SerializeField] private float sightRange;
     
        [Header("1.0: Guard")] 
        // will unmovingly guard a point. Shoot any enemy they see. If somehow moved, try to get back to the point;
        [SerializeField] private Transform guardPoint;
        [SerializeField] private Vector2 tolerance;
        
        [Header("2.0: Chaser")] // currently non-functioning
        // will hunt down the target as long as it is in sight range. Otherwise, it will reach the target's last seen position and remain there;
        [SerializeField] private Transform target;
        [SerializeField] private Vector2 lastSeenPosition;
        private bool _seesTarget;
        
        
    
    private Transform _target;
    private bool _moving = false;
    
    
    protected override void Start(){
        base.Start();

        if (guardPoint == null && aiType < 2 && aiType >= 1){
            Debug.LogError("Haven't assigned this guard a guard point");
            
        }
        
        
    }

    private void AiLogic(){
        if (aiType == 1.0f){
            _moving = false;
            if (Math.Abs(transform.position.x - guardPoint.position.x) > tolerance.x || Math.Abs(transform.position.y - guardPoint.position.y) > tolerance.y){
                _target = guardPoint;
                _moving = true;
            }
        }
        else if ( aiType == 2.0f){
            RaycastHit2D raycast = Physics2D.Raycast(transform.position, target.transform.position, sightRange, LayerMask.GetMask("Ground", "Team 1", "Team 2", "Team 3", "Team 4"));
            if (!(raycast.collider is null)){
                target = raycast.collider.transform;
                lastSeenPosition = target.transform.position;
                _seesTarget = true;
            }
            else{
                _seesTarget = false;
            }
        }
    }
    
    
    protected override void FixedUpdate(){
        base.FixedUpdate();
        
        AiLogic();
        if (!_moving){
            return;
        }
        Pathfinding();
    }


    private void Pathfinding(){
        
        int direction = Math.Sign(_target.transform.position.x - transform.position.x);
        
        if (!InputsFrozen && !Knocked){
            Body.velocity = new Vector2(moveSpeed * direction, Body.velocity.y);
        }
    }
    
    
    /*
    [SerializeField] private float aiType;
    
        [Header("1.0: Guard")] 
        // will unmovingly guard a point. Shoot any enemy they see. If somehow moved, try to get back to the point;
        [SerializeField] private Transform guardPoint;
        [SerializeField] private Vector2 tolerance;
        
    
    private Transform _target;
    private float _nextWaypointDistance = 3;
    private int _currentWaypoint = 0;
    private bool _reachedEndofPath = false;
    private bool _moving = false;
    
    private Path _path;
    private Seeker _seeker;
    
    
    protected override void Start(){
        base.Start();
        _seeker = GetComponent<Seeker>();

        if (guardPoint == null && aiType < 2 && aiType >= 1){
            Debug.LogError("Haven't assigned this guard a guard point");
        }
        
        
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
    }

    private void AiLogic(){
        if (aiType == 1.0f){
            _moving = false;
            if (Math.Abs(transform.position.x - guardPoint.position.x) > tolerance.x || Math.Abs(transform.position.y - guardPoint.position.y) > tolerance.y){
                _target = guardPoint;
                _moving = true;
            }
        }
    }

    private bool Exit(){
        if (!_moving || _path == null){
            return true;
        }

        if (_currentWaypoint >= _path.vectorPath.Count){
            _reachedEndofPath = true;
            return true;
        }

        
        return false;
    }
    
    protected override void FixedUpdate(){
        base.FixedUpdate();
        
        AiLogic();
        if (Exit()){
            return;
        }

        int direction = Math.Sign(_path.vectorPath[_currentWaypoint].x - transform.position.x);
        
        if (!_reachedEndofPath && !InputsFrozen && !Knocked){
            Body.velocity = new Vector2(moveSpeed * direction, Body.velocity.y);
        }
        
        CheckNextWaypoint();
    }

    private void CheckNextWaypoint(){
        float distance = Vector2.Distance(transform.position, _path.vectorPath[_currentWaypoint]);
        if (distance < _nextWaypointDistance){
            _currentWaypoint++;
        }
    }

    private void UpdatePath(){
        if (_moving){
            if (_seeker.IsDone()){
                _seeker.StartPath(transform.position, _target.position, OnPathComplete);
                
            }
        }
    }
    

    private void OnPathComplete(Path p){
        if (!p.error){
            _path = p;
            _currentWaypoint = 1;
            _reachedEndofPath = false;
        }
    }*/
}
    
