
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
    [Header("Player")]
    public VirtualPlayer virtualPlayer;
    public int teamIndex;
    private ModeManager _modeManager;
    
    [Client]
    public override void OnStartClient(){
        ClientHUDVisualStart();
        ClientWeaponControlStart();
        
        base.OnStartClient();
    }
    

    [Server]
    public override void OnStartServer(){
        _modeManager = FindObjectOfType<ModeManager>();
    }


    [Client]
    protected override void ClientUpdate(){
        if (!_dead){
            base.ClientUpdate();

            _lastArmAngle = GetBarrelToMouseRotation();

            ClientMoveUpdate();
            ClientPlayerAbilitiesUpdate(); // all hud and audio stuff
            ClientPlayerWeaponUpdate();

            if (Input.GetKeyDown(KeyCode.O)){
                CmdDie();
            }
        }

        ClientHUDUpdate();
        
        
    }
    
    
    [Client]
    protected override void ClientFixedUpdate(){
        if (!_dead){
            base.ClientFixedUpdate();

            _lastArmAngle = GetBarrelToMouseRotation();

            ClientMoveFixedUpdate();
            ClientPlayerWeaponFixedUpdate();
            ClientPlayerAbilitiesFixedUpdate();

            _currentInput.CrouchInput = _isCrouching;
            _currentInput.SprintInput = _isSprinting;
            _currentInput.RequestNumber = _inputRequestCounter;
            ClientSendServerInputs();
        }
    }
  
    
    
    [Server]
    protected override void ServerFixedUpdate(){
        if (!_dead){
            base.ServerFixedUpdate();
            ServerPlayerNetworkedMovementFixedUpdate();
            ServerPlayerCombatFixedUpdate();
            ServerPlayerWeaponFixedUpdate();
            ServerPlayerAbilitiesFixedUpdate();
        }
    }
    

    #region Animation
    
    
    private readonly ANames _aNames = new ANames();

    
    [Client]
    private void ClientSetAnimatedBoolOnAll(string animationName, bool setTo){
        animator.SetBool(animationName, setTo);
        CmdSetAnimatedBoolOnServer(animationName, setTo);
    }

    [Command]
    private void CmdSetAnimatedBoolOnServer(string animationName, bool setTo){
        SetAnimatedBoolOnClientRpc(animationName, setTo);
    }

    [ClientRpc]
    private void SetAnimatedBoolOnClientRpc(string animationName, bool setTo){
        animator.SetBool(animationName, setTo);
    }

    [Client]
    private void ClientSetAnimatorSpeedOnAll(float speed){
        animator.speed = speed;
        CmdSetAnimatedBoolOnServer(speed);
    }

    [Command]
    private void CmdSetAnimatedBoolOnServer(float speed){
        SetAnimatorSpeedClientRpc(speed);
    }

    [ClientRpc]
    private void SetAnimatorSpeedClientRpc(float speed){
        animator.speed = speed;
    }
    
    private void Transform(float x){ // for animation events
        Vector2 pos = transform.position;
        transform.position = new Vector3(pos.x + x * _direction, pos.y);
    }

    #endregion
    
    
    

}
