using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hook : MonoBehaviour{
    public Rigidbody2D body;
    private GrapplingHook _firer;
    [SerializeField] private SpringJoint2D springJoint;

    public float saveMovementSpeed;
    private void OnCollisionEnter2D(Collision2D col){
        Debug.Log(col.gameObject.name);

        if (_firer.state == 1){
            // attaches the hook to the object it hits
            body.bodyType = RigidbodyType2D.Static;
            transform.parent = col.transform;
            _firer.state = 2;

            springJoint.enabled = true;
            springJoint.connectedBody = _firer.WeaponAttachedTo.wielder.body;
            springJoint.distance = 0;
            saveMovementSpeed = _firer.WeaponAttachedTo.wielder.moveSpeed;
            _firer.WeaponAttachedTo.wielder.moveSpeed = 0;
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
        _firer.WeaponAttachedTo.wielder.moveSpeed = saveMovementSpeed;
    }

    private void FixedUpdate(){
        if (_firer.state == 3){
            body.velocity = (_firer.transform.position - transform.position).normalized * 70;
        }
    }

    public void SetFirer(GrapplingHook firer){
        _firer = firer;
        saveMovementSpeed = firer.WeaponAttachedTo.wielder.moveSpeed;
    }

    private void Start(){
        springJoint.enabled = false;
    }
}
