using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;
using UnityEngine.XR;

public partial class Player : Character{
    
    /*
* ================================================================================================================
*                                               Player
*
 * contains all player logic. Movement, weapons, grenades, swapping weapons, animation, sounds, and additional foot collider
*
* 
* ================================================================================================================
*/
    
    
    [Client]
    public override void OnStartClient(){
        
        ClientHUDVisualStart();
        ClientAbilityStart();
        ClientWeaponControlStart();

        base.OnStartClient();

    }
    
  
    [Client]
    protected override void ClientUpdate(){
        base.ClientUpdate();
        
        _lastArmAngle = GetBarrelToMouseRotation();
        
        ClientMoveUpdate();
        ClientPlayerAbilitiesUpdate(); // all hud and audio stuff
        ClientPlayerWeaponUpdate();
        ClientHUDUpdate();
        
        
    }
    
    
    [Client]
    protected override void ClientFixedUpdate(){
        base.ClientFixedUpdate();
        
        _lastArmAngle = GetBarrelToMouseRotation();
        
        ClientMoveFixedUpdate();
        
        ClientSendServerInputs(_currentInput, _currentPress);
    }
  
    
    
    [Server]
    protected override void ServerFixedUpdate(){
        base.ServerFixedUpdate();

        ServerPlayerNetworkedMovementFixedUpdate();
        ServerAbilitiesFixedUpdate();
        ServerPlayerWeaponFixedUpdate();
        

        
    }


    
    public override void Hit(int damage){
        base.Hit(damage);
    }


    #region Send Info to Server


    [Client]
    private void ClientSendServerInputs(PlayerInput playerInput, PlayerKeyPresses keyPresses){
        if (hasAuthority){
            _pastInputs.Add(playerInput); // remember all inputs for later client prediction
            _pastPresses.Add(keyPresses);

            CmdSetServerValues(playerInput.HorizontalInput, playerInput.Rotation, playerInput.ArmRotationInput,
                playerInput.RequestNumber, keyPresses.JumpInput, keyPresses.CrouchInput, keyPresses.ReloadInput);

            _inputRequestCounter++;

            _currentInput = new PlayerInput();
            _currentPress = new PlayerKeyPresses();
        }
    }


    [Command] // server remembers only the most recent inputs
    private void CmdSetServerValues(float lastHorizontalInput, float lastRotationInput, float lastArmRotation, int requestCounter, bool jumpInput, bool crouchInput, bool reloadInput){
        // remembers to use later in server's fixed update
        _lastInput = new PlayerInput(lastHorizontalInput, lastRotationInput, lastArmRotation, requestCounter);
        
        _lastPress = new PlayerKeyPresses(jumpInput, crouchInput, reloadInput, requestCounter);
        if (jumpInput){
            Jump();
        }


        if (reloadInput){
            primaryWeapon.CmdReload();
        }

    }
    #endregion


    #region Animation
    
    private readonly ANames _aNames = new ANames();

    
    [Client]
    private void ClientSetAnimatedBoolOnAll(string animationName, bool setTo){
        Animator.SetBool(animationName, setTo);
        CmdSetAnimatedBoolOnServer(animationName, setTo);
    }

    [Command]
    private void CmdSetAnimatedBoolOnServer(string animationName, bool setTo){
        SetAnimatedBoolOnClientRpc(animationName, setTo);
    }

    [ClientRpc]
    private void SetAnimatedBoolOnClientRpc(string animationName, bool setTo){
        Animator.SetBool(animationName, setTo);
    }
    
    private void Transform(float x){ // for animation events
        Vector2 pos = transform.position;
        transform.position = new Vector3(pos.x + x * transform.localScale.x, pos.y);
    }

    #endregion


    #region Input Stuff

    
    //for server to know where client's trying to go
    private PlayerInput _lastInput;
    private PlayerKeyPresses _lastPress;
    
    // for client predictive movement
    private int _inputRequestCounter; 
    
    
    private List<PlayerInput> _pastInputs = new List<PlayerInput>();
    private PlayerInput _currentInput;

    private List<PlayerKeyPresses> _pastPresses = new List<PlayerKeyPresses>();
    private PlayerKeyPresses _currentPress;

    
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

    #endregion

}
