using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public partial class Player : Character{
    
    private float _lastHorizontalInput;
    private bool _lastInputJumped;
    private float _lastRotationInput;
    
    private int _requestCounter;

    
    [Command]
    private void CmdSetServerValues(float lastHorizontalInput, bool lastInputJumped, float lastRotationInput, int requestCounter){
        _lastHorizontalInput = lastHorizontalInput;
        _lastInputJumped = lastInputJumped;
        _lastRotationInput = lastRotationInput;
        if (lastInputJumped){
            ServerJump();
        }
    }

    [ClientRpc]
    private void SetClientPositionRpc(Vector3 position, Quaternion rotation, Vector3 scale, Vector2 velocity, int requestCounter){
        
        transform.position = position;
        transform.rotation = rotation;
        
        body.velocity = velocity;
    }


    private void ClientMoveUpdate(){
        if (Input.GetKeyDown(KeyCode.W)){
            Jump();
            CmdSetServerValues(Input.GetAxis("Horizontal"), true, 0, _requestCounter);
            
            SetAnimatedBoolOnAll(_aNames.jumping, true);
            SetAnimatedBoolOnAll(_aNames.crouching, false);
            _isCrouching = false;
        }

        if (Input.GetKeyDown(KeyCode.S)){
            _isCrouching = !_isCrouching;
            SetAnimatedBoolOnAll(_aNames.crouching, _isCrouching);
        }
    }

    private void ClientMoveFixedUpdate(){
        if (hasAuthority){
            float input = Input.GetAxis("Horizontal");
            SetAnimatedBoolOnAll(_aNames.running, input != 0);
            SetAnimatedBoolOnAll(_aNames.runningBackwards, Math.Sign(input) != Math.Sign(transform.localScale.x));
            Move(input);
            RotateArm(GetBarrelToMouseRotation());
            CmdRotateArm(GetBarrelToMouseRotation());
            CmdSetServerValues(input, false, 0, _requestCounter);
        }
    }

    private void ServerFixedMove(){ // fixed update
        
        Move(_lastHorizontalInput);

    }

    [ServerCallback]
    private void LateUpdate(){
        SetClientPositionRpc(transform.position, transform.rotation, transform.localScale, body.velocity, _requestCounter);
    }

    private void Move(float input){
        if (input != 0 && !InputsFrozen && !FallingKnocked){

            if (_isCrouching){
                body.velocity = new Vector2(moveSpeed / 2 * input, body.velocity.y);
                SortSound(2);
            }
            else{
                body.velocity = new Vector2(moveSpeed * input, body.velocity.y);
            }
        }

    }

    private void ServerJump(){
        Jump();
        SetClientPositionRpc(transform.position, transform.rotation, transform.localScale, body.velocity, _requestCounter);

    }

    private void Jump(){
        
        Vector2 velocity = body.velocity;
        if (FeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground", "Platform", "Vehicle", "Vehicle Outer", "Team 1", "Team 2", "Team 3", "Team 4"))){
            Airborne = true;
            body.velocity = new Vector2(velocity.x, jumpVelocity);
            SortSound(0); 

            /*
            the player has a little bit of area where they're not technically touching the ground but need to act like it 
            for the game to look good like when going down a slope. Because of this, right after jumping the player is still
            inside this area, so the ground check needs to be temporarily suppresses right after jumping
            */
            StartCoroutine(ResetGroundCheck());
        }
    }
    
    
    
    
    [Command]
    private void CmdRotateArm(float rotation){
        RotateArmClientRpc(rotation);
    }

    [ClientRpc]
    private void RotateArmClientRpc(float rotation){
        RotateArm(rotation);
    }

    private void RotateArm(float rotation){
        if (_armOverride == false){
            arm.transform.rotation = Quaternion.Euler(0, 0, rotation);
            arm.transform.localScale = new Vector3(1, 1);
        }
        else{
            arm.transform.localRotation = Quaternion.Euler(0, 0, -30);
            if (Mathf.Sign(transform.localScale.x) < 0){
                arm.transform.rotation = Quaternion.Euler(0, 0, -150);
    
            }
            arm.transform.localScale = new Vector3(1, 1);
        }

        helmet.transform.rotation = Quaternion.Euler(0, 0, rotation);
        helmet.transform.localScale = new Vector3(1, 1);


        if (rotation > 90 && rotation < 270){
            arm.transform.localScale = new Vector3(-1, -1);
            helmet.transform.localScale = new Vector3(-1, -1);
            transform.localScale = new Vector3(-1, 1);
        }
        else{
            transform.localScale = new Vector3(1, 1);
        }
    }
}

