
using System.Collections.Generic;
using Mirror;
using UnityEngine;

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
        ClientPlayerWeaponFixedUpdate();
        
        _currentInput.CrouchInput = _isCrouching;
        _currentInput.RequestNumber = _inputRequestCounter;
        ClientSendServerInputs();

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
    private void ClientSendServerInputs(){
        if (hasAuthority){
            _pastInputs.Add(_currentInput); // remember all inputs for later client prediction
            CmdSetServerValues(_currentInput);

            _inputRequestCounter++;
            _currentInput = new PlayerInput();
        }
    }



    [Command] // server remembers only the most recent inputs
    private void CmdSetServerValues(PlayerInput playerInput){
        // remembers to use later in server's fixed update
        _lastInput = playerInput;
        
        
        if (playerInput.JumpInput){
            Jump();
        }

        _isCrouching = playerInput.CrouchInput;

        if (playerInput.ReloadInput){
            primaryWeapon.Reload();
        }

        if (playerInput.SwapWeapon){
            PlayerSwapWeapon();
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
    
    // for client predictive movement
    private int _inputRequestCounter; 
    private List<PlayerInput> _pastInputs = new List<PlayerInput>();
    private PlayerInput _currentInput;
    
    
    // a simple container for some information
    private struct PlayerInput{ // for constant things
        public float HorizontalInput;
        public float Rotation;
        public float ArmRotationInput;
        
        public bool JumpInput;
        public bool CrouchInput;
        
        public bool FiringInput;
        public float FiringAngle;
        public bool ReloadInput;

        public bool SwapWeapon;
        
        public int RequestNumber;
    }
 

    #endregion


}
