using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public partial class Player : Character{

    [Header("Movement")]
    [SerializeField] private float sprintAmplifier;
    
    private bool _isCrouching;
    private bool _isSprinting;
    private int _direction;

    private bool _hardLanding; // used for sound


    [Client] // handles key presses like jumping and animations
    private void ClientMoveUpdate(){
        if (hasAuthority){
            if (Input.GetKeyDown(KeyCode.W)){
                Jump();
                _currentInput.JumpInput = true;

                ClientSetAnimatedBoolOnAll(_aNames.jumping, true);
                ClientSetAnimatedBoolOnAll(_aNames.crouching, false);
            }

            if (Input.GetKeyDown(KeyCode.S)){
                _isCrouching = !_isCrouching;

                
                ClientSetAnimatedBoolOnAll(_aNames.crouching, _isCrouching);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift)){
                _isSprinting = true;
                _isCrouching = false;
                ClientSetAnimatorSpeedOnAll(sprintAmplifier * (moveSpeed / 15));
            }

            if (Input.GetKeyUp(KeyCode.LeftShift)){
                _isSprinting = false;
                ClientSetAnimatorSpeedOnAll(moveSpeed / 15f);
            }
            
        }

        ClientSetAnimatedBoolOnAll(_aNames.jumping, Airborne);
        ClientCheckForHardLanding();
    }

    private void ClientMoveFixedUpdate(){ // handles main movement, send inputs to server
        if (hasAuthority){
            float input = Input.GetAxis("Horizontal");
            ClientSetAnimatedBoolOnAll(_aNames.running, input != 0);
            ClientSetAnimatedBoolOnAll(_aNames.runningBackwards, Math.Sign(input) != _direction);
            

            Move(input);
            RotateArm(_lastArmAngle);
            _currentInput.HorizontalInput = input;
            _currentInput.Rotation = 0;
            _currentInput.ArmRotationInput = _lastArmAngle;
            
            
            
        }
    }
     // all inputs in the fixed update frame are registered as currentInput
     // all keypresses in the update frame are registered as currentPress
     // at the end of fixedUpdate and Update, they're collectively sent to the server at once using methods in Player

     
     
    [Server] // every physics update uses latest input to move player
    private void ServerPlayerNetworkedMovementFixedUpdate(){ // fixed update
        
        Move(_lastInput.HorizontalInput);
       
        RotateArm(_lastInput.ArmRotationInput);
    }
    
    

    #region Dumb movement

    // these are called by server for validation and by client to look good. Nothing special happening here

    private void Move(float input){
        if (input != 0 && !InputsFrozen && !fallingKnocked){
            if (_isCrouching){
                body.velocity = new Vector2(moveSpeed / 2 * input, body.velocity.y);
                SortSound(2);
            }
            else{
                if (_isSprinting){
                    primaryWeapon.ResetZoom();
                    _attemptingToFire = false;
                    _firingAngle = 0;
                    body.velocity = new Vector2(moveSpeed * sprintAmplifier * input, body.velocity.y);
                }
                else{
                    body.velocity = new Vector2(moveSpeed * input, body.velocity.y);
                }
            }
        }

    }
    
    private void Jump(){
        
        Vector2 velocity = body.velocity;
        if (!Airborne){
            Airborne = true;
            _isCrouching = false;
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
        if (!_armOverrideReloading){
            arm.transform.rotation = Quaternion.Euler(0, 0, rotation);
            arm.transform.localScale = new Vector3(1, 1);
        }
        else{
            arm.transform.rotation = Quaternion.Euler(0, 0, -30); // -30
            if (Mathf.Sign(transform.localScale.x) < 0){
                arm.transform.rotation = Quaternion.Euler(0, 0, -150); // -150
    
            }
            arm.transform.localScale = new Vector3(1, 1);
        }

        helmet.transform.rotation = Quaternion.Euler(0, 0, rotation);
        helmet.transform.localScale = new Vector3(1, 1);
        

        if (rotation > 90 && rotation < 270){
            arm.transform.localScale = new Vector3(-1, -1);
            helmet.transform.localScale = new Vector3(-1, -1);
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
            _direction = -1;
        }
        else{
            transform.localScale = new Vector3( Mathf.Abs(transform.localScale.x), transform.localScale.y);
            _direction = 1;
        }
    }

    
    #endregion

    private void CheckSoundsOnCollision(Collision2D other){
        if (other.gameObject.CompareTag("Ground")){
            if (_hardLanding){
                SortSound(4);
            }
            else{
                SortSound(3);
            }
        }
    }

    private void ClientCheckForHardLanding(){ // only for audio
        
        if (Math.Abs(body.velocity.x) > 20 || Math.Abs(body.velocity.y) > 70){
            _hardLanding = true;
        }
        else{
            _hardLanding = false;
        }
    }

    
}

