using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class Player : Character{


    
    private bool _isCrouching;

    [SyncVar] private bool _hardLanding; // used for sound
    

    //for server to know where client's trying to go
    private PlayerInput _lastInput;
    private PlayerKeyPresses _lastPress;
    
    // for client predictive movement
    private int _inputRequestCounter; 
    private List<PlayerInput> _pastInputs = new List<PlayerInput>();
    private PlayerInput _currentInput;

    private int _pressRequestCounter;
    private List<PlayerKeyPresses> _pastPresses = new List<PlayerKeyPresses>();
    private PlayerKeyPresses _currentPress;


    [Client] // handles key presses like jumping and animations
    private void ClientMoveUpdate(){
        
        if (Input.GetKeyDown(KeyCode.W)){
            Jump();
            _currentPress.JumpInput = true;
            
            ClientSetAnimatedBoolOnAll(_aNames.jumping, true);
            ClientSetAnimatedBoolOnAll(_aNames.crouching, false);
        }

        if (Input.GetKeyDown(KeyCode.S)){
            _isCrouching = !_isCrouching;
            _currentPress.CrouchInput = _isCrouching;
            ClientSetAnimatedBoolOnAll(_aNames.crouching, _isCrouching);
        }
        ClientSetAnimatedBoolOnAll(_aNames.jumping, Airborne);
        ClientCheckForHardLanding();

    }

    private void ClientMoveFixedUpdate(){ // handles main movement, send inputs to server
        if (hasAuthority){
            float input = Input.GetAxis("Horizontal");
            ClientSetAnimatedBoolOnAll(_aNames.running, input != 0);
            ClientSetAnimatedBoolOnAll(_aNames.runningBackwards, Math.Sign(input) != Math.Sign(transform.localScale.x));
            Move(input);
            RotateArm(_lastArmAngle);
            _currentInput.HorizontalInput = input;
            _currentInput.Rotation = 0;
            _currentInput.ArmRotationInput = _lastArmAngle;
            _currentInput.RequestNumber = _inputRequestCounter;
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

    // just called directly when info is received
    private void ServerJump(){
        Jump();
        ServerRefreshPositionForClients();
    }
    
    // after update, sends information back to clients    
    [ServerCallback]
    private void LateUpdate(){
        ServerRefreshPositionForClients();
    }

    
    // shorthand since all arguments are same. Sends all current information after physics solve it to all clients
    [Server]
    private void ServerRefreshPositionForClients(){
        SetClientPositionRpc(transform.position, transform.rotation, transform.localScale, _lastInput.ArmRotationInput, body.velocity, _lastInput.RequestNumber);
    }

    
    // the clients receive the information and update themselves accordingly
    [ClientRpc]
    private void SetClientPositionRpc(Vector3 position, Quaternion rotation, Vector3 scale, float armRotation, Vector2 velocity, int requestCounter){

        // if too far away or not controlled by this player then instantly update the position
        if (hasAuthority && Vector3.Distance(transform.position, position) > 2 || !hasAuthority){
            transform.position = position;
            transform.rotation = rotation;
            RotateArm(armRotation); // fancier way of doing scale
            body.velocity = velocity;
        }
        
        
        if (hasAuthority){
            // if not too far away then reconcile with server by remembering all previous inputs and simulating them from server
            ClientPositionReconciliation(requestCounter);
        }
    }

    
    [Server]
    private void ServerRefreshStatesForClients(PlayerKeyPresses playerKeyPresses){
        SetClientStatesRpc(playerKeyPresses.JumpInput, playerKeyPresses.CrouchInput, playerKeyPresses.ReloadInput);
    }

    [ClientRpc]
    private void SetClientStatesRpc(bool jumpInput, bool crouchInput, bool reloadInput){
        
    }

    
    // this is where most of the magic happens, it's hard to explain
    [Client]
    private void ClientPositionReconciliation(int requestCounter){
        
        List<PlayerInput> inputsToRemove = new List<PlayerInput>();

        // figure out what the server knows and what needs to be resimulated
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
            

            Physics.Simulate(Time.fixedDeltaTime);
        }
        
        Physics.autoSimulation = true;
    }

    [Client]
    private void ClientStateReconciliation(int requestCounter){
        
    }


    #region Dumb movement

    // these are called by server for validation and by client to look good. Nothing special happening here

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
    
    private void Jump(){
        
        Vector2 velocity = body.velocity;
        if (FeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground", "Platform", "Vehicle", "Vehicle Outer", "Team 1", "Team 2", "Team 3", "Team 4"))){
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

    
    #endregion
    
    
    [ClientCallback]
    protected override void OnCollisionEnter2D(Collision2D other){ // technically this has a part of combat, but that's handled in the parent so no need for a seperate combat partial
        base.OnCollisionEnter2D(other);
        
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

    // a simple container for some information
    private struct PlayerInput{ // for constant things
        public float HorizontalInput;
        public float Rotation;
        public float ArmRotationInput;
        
        public int RequestNumber;
        
        public PlayerInput(float horizontalInput, float rotation, float armRotationInput, int requestNumber){
            HorizontalInput = horizontalInput;
            Rotation = rotation;
            ArmRotationInput = armRotationInput;
            
            RequestNumber = requestNumber;
        }
    }

    private struct PlayerKeyPresses{ // for button presses
        public bool JumpInput;
        public bool CrouchInput;
        public bool ReloadInput;

        public int RequestNumber;

        public PlayerKeyPresses(bool jumpInput, bool crouchInput, bool reloadInput, int requestNumber){
            JumpInput = jumpInput;
            CrouchInput = crouchInput;
            ReloadInput = reloadInput;

            RequestNumber = requestNumber;
        }
    }
    
}
