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
        
        ClientSendServerKeyPresses(_currentPress);
        
    }
    
    
    [Client]
    protected override void ClientFixedUpdate(){
        base.ClientFixedUpdate();
        
        _lastArmAngle = GetBarrelToMouseRotation();
        
        ClientMoveFixedUpdate();
        
        ClientSendServerInputs(_currentInput);

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
    private void ClientSendServerInputs(PlayerInput playerInput){
        _pastInputs.Add(playerInput); // remember all inputs for later client prediction
        CmdSetServerValues(playerInput.HorizontalInput, playerInput.Rotation, playerInput.ArmRotationInput, playerInput.RequestNumber);
        _inputRequestCounter++;
    }

    [Client]
    private void ClientSendServerKeyPresses(PlayerKeyPresses keyPresses){
        _pastPresses.Add(new PlayerKeyPresses(keyPresses.JumpInput, keyPresses.CrouchInput, keyPresses.CrouchInput, keyPresses.RequestNumber));
        CmdSetServerPresses(keyPresses.JumpInput, keyPresses.CrouchInput, keyPresses.ReloadInput);
        _pressRequestCounter++;
    }
    
    
    [Command] // server remembers only the most recent inputs
    private void CmdSetServerValues(float lastHorizontalInput, float lastRotationInput, float lastArmRotation, int requestCounter){
        // remembers to use later in server's fixed update
        _lastInput = new PlayerInput(lastHorizontalInput, lastRotationInput, lastArmRotation, requestCounter);
    }

    
    [Command]
    private void CmdSetServerPresses(bool jumpInput, bool crouchInput, bool reloadInput){
        _lastPress = new PlayerKeyPresses(jumpInput, crouchInput, reloadInput);
        // same but since they're right now things they can be handled right now
        if (jumpInput){
            ServerJump();
        }

        _isCrouching = crouchInput;

        if (reloadInput){
            primaryWeapon.CmdReload();
        }
        
        ServerRefreshStatesForClients();
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


}
