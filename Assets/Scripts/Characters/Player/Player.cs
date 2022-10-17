
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
        ClientWeaponControlStart();
        base.OnStartClient();
    }

    [Server]
    public override void OnStartServer(){
        ServerPlayerAbilitiesStart();
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
        ServerPlayerCombatFixedUpdate();
        ServerAbilitiesFixedUpdate();
        ServerPlayerWeaponFixedUpdate();
        
        
    }
    

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
