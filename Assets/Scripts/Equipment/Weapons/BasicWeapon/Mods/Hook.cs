using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour{
    public SpringJoint2D springJoint;
    public Rigidbody2D body;
    private GrapplingHook _firer;
    private Player _player;
    [SerializeField] private LineRenderer lineRenderer;

    public bool locked;

    public float saveMovementSpeed;
    private void OnCollisionEnter2D(Collision2D col){

        if (_firer.state == 1){
            // attaches the hook to the object it hits
            body.bodyType = RigidbodyType2D.Static;
            transform.parent = col.transform;
            _firer.state = 2;

            springJoint.enabled = true;
            springJoint.connectedBody = _player.body;
            if (locked){
                springJoint.distance = Vector2.Distance(transform.position, _player.transform.position);
            }
            else{
                springJoint.distance = 0;    
            }
            saveMovementSpeed = _player.moveSpeed;
            _player.moveSpeed = 0;
        }
    }

    public void Disengage(){
        body.bodyType = RigidbodyType2D.Dynamic;
        transform.parent = null;
        springJoint.enabled = false;
        gameObject.layer = LayerMask.NameToLayer("Default"); 
        ResetHook();
    }

    public void ResetHook(){
        _player.moveSpeed = saveMovementSpeed;
        springJoint.enabled = false;
        locked = false;
    }

    private void FixedUpdate(){
        if (_firer.state == 3){
            body.velocity = (_firer.transform.position - transform.position).normalized * 70;
        }
    }

    private void Update(){
        if (_firer.state != 0){
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, _player.transform.position);
        }
    }

    public void SetFirer(GrapplingHook firer){
        _firer = firer;
        _player = _firer.WeaponAttachedTo.wielder;
        saveMovementSpeed = _player.moveSpeed;
    }

    private void Start(){
        springJoint.enabled = false;
        lineRenderer.useWorldSpace = true;
    }
}
