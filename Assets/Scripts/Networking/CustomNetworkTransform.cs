using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Player : Character{
    
    private float _lastHorizontalInput; // for server
    private bool _lastInputJumped; // for server
    private float _lastRotationInput; // for server
    private float _lastArmRotation; // for server
    private int _lastRequest; // for server
    
    private int _requestCounter; // for client

    private List<PlayerInput> _pastInputs = new List<PlayerInput>();

    private float _lastArmAngle; // client only so barrel to mouse isn't constantly recalculated


    
    [Command]
    private void CmdSetServerValues(float lastHorizontalInput, bool lastInputJumped, float lastRotationInput, float lastArmRotation, int requestCounter){
        _lastHorizontalInput = lastHorizontalInput;
        _lastInputJumped = lastInputJumped;
        _lastRotationInput = lastRotationInput;
        _lastArmRotation = lastArmRotation;
        _lastRequest = requestCounter;
        if (lastInputJumped){
            ServerJump();
        }
    }

    [ClientRpc]
    private void SetClientPositionRpc(Vector3 position, Quaternion rotation, Vector3 scale, float armRotation, Vector2 velocity, int requestCounter){
        
        transform.position = position;
        transform.rotation = rotation;
        RotateArm(armRotation); // fancier way of doing scale
        body.velocity = velocity;
        ;
        if (hasAuthority){
            List<PlayerInput> inputsToRemove = new List<PlayerInput>();
            foreach (PlayerInput playerInput in _pastInputs){
                if (playerInput.RequestNumber < requestCounter){
                    inputsToRemove.Add(playerInput);
                }
            }

            foreach (PlayerInput toRemove in inputsToRemove){
                _pastInputs.Remove(toRemove);
            }
            
            // at this stage, you have a list of every input since the last one confirmed by the server
            // now you want to loop over every past input and simulate physics using it
            Physics.autoSimulation = false;
            foreach (PlayerInput pastInput in _pastInputs){
                Move(pastInput.HorizontalInput);
                if (pastInput.JumpInput){
                    Jump();
                }

                Physics.Simulate(Time.fixedDeltaTime);
            }
            Physics.autoSimulation = true;
        }

    }


    private void ClientMoveUpdate(){
        
        if (Input.GetKeyDown(KeyCode.W)){
            Jump();
            ClientSendServerInputs(Input.GetAxis("Horizontal"), true, 0, _lastArmAngle, _requestCounter);
            
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
            RotateArm(_lastArmAngle);
            ClientSendServerInputs(input, false, 0, _lastArmAngle, _requestCounter);
        }
    }

    private void ServerFixedMove(){ // fixed update
        
        Move(_lastHorizontalInput);
        RotateArm(_lastArmRotation);

    }

    [ServerCallback]
    private void LateUpdate(){
        ServerRefreshForClients();
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

    [Server]
    private void ServerRefreshForClients(){
        SetClientPositionRpc(transform.position, transform.rotation, transform.localScale, _lastArmRotation, body.velocity, _lastRequest);
    }

    private void ServerJump(){
        Jump();
        ServerRefreshForClients();
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

    private void ClientSendServerInputs(float horizontalInput, bool jumpInput, float rotation, float armRotationInput, int requestNumber){
        _pastInputs.Add(new PlayerInput(horizontalInput, jumpInput, rotation, armRotationInput, requestNumber));
        CmdSetServerValues(horizontalInput, jumpInput, rotation, armRotationInput, requestNumber);
        _requestCounter++;
    }

    private class PlayerInput{
        public readonly float HorizontalInput;
        public readonly bool JumpInput;
        public readonly float Rotation;
        public readonly float ArmRotationInput;
        public readonly float RequestNumber;

        
        public PlayerInput(float horizontalInput, bool jumpInput, float rotation, float armRotationInput, int RequestNumber){
            HorizontalInput = horizontalInput;
            JumpInput = jumpInput;
            Rotation = rotation;
            ArmRotationInput = armRotationInput;
            this.RequestNumber = RequestNumber;
        }
    }
}

