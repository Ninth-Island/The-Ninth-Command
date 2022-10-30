
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public partial class Player : Character{
 
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

        _isCrouching = playerInput.CrouchInput && !playerInput.SprintInput;
        _isSprinting = playerInput.SprintInput;

        _isArmorAbilitying = playerInput.AbilityPressed;
        _isModAbilitying = playerInput.ModPressed;
        
        if (playerInput.ReloadInput){
            primaryWeapon.Reload();
        }

        if (playerInput.AbilityInput){
            ArmorAbilityInstant();
        }

        if (playerInput.ModInput){
            ModAbilityInstant();
        }

        if (playerInput.SwapWeapon){
            PlayerSwapWeapon();
            PlayerSwapWeaponClientRpc();
            
        }

        if (playerInput.PickedUp){
            if (Vector2.Distance(playerInput.PickedUp.transform.position, transform.position) < 14 && playerInput.PickedUp.gameObject.layer == LayerMask.NameToLayer("Objects")){

                playerInput.PickedUp.SwapTo(this, playerInput.OldWeapon, new[]{1, 3});
                PlayerPickUpWeaponClientRpc(playerInput.PickedUp, playerInput.OldWeapon);
                
                playerInput.PickedUp.netIdentity.AssignClientAuthority(connectionToClient);
                playerInput.OldWeapon.netIdentity.RemoveClientAuthority();
                
            }
            else{
                playerInput.OldWeapon.CancelPickup(this, new []{1, 3});
            }
        }
    }
    
    [ClientRpc]
    private void PlayerPickUpWeaponClientRpc(BasicWeapon newWeapon, BasicWeapon oldWeapon){
        if (!hasAuthority){
            primaryWeapon.StopReloading();        
            FinishReload();

            newWeapon.SwapTo(this, oldWeapon, new[]{1, 3});
            SetArmType(primaryWeapon.armType);
        }
    }


    // after update, sends information back to clients    
    [ServerCallback]
    private void LateUpdate(){
        ServerOverrideActualValuesForClients();
    }

    // shorthand since all arguments are same. Sends all current information after physics solve it to all clients
    [Server]
    private void ServerOverrideActualValuesForClients(){
        SetClientValuesRpc(transform.position, transform.rotation, transform.localScale, _lastInput.ArmRotationInput, body.velocity, _currentAbilityCharge, _lastInput.RequestNumber);
    }

    
    // the clients receive the information and update themselves accordingly
    [ClientRpc]
    private void SetClientValuesRpc(Vector3 position, Quaternion rotation, Vector3 scale, float armRotation, Vector2 velocity, int currentAbilityCharge, int requestCounter){

        // if too far away or not controlled by this player then instantly update the position
        if (hasAuthority && Vector3.Distance(transform.position, position) > 2 || !hasAuthority){
            transform.position = position;
            transform.rotation = rotation;
            RotateArm(armRotation); // fancier way of doing scale
            body.velocity = velocity;
            _currentAbilityCharge = currentAbilityCharge;
        }

        if (hasAuthority){
            // if not too far away then reconcile with server by remembering all previous inputs and simulating them from server
            ClientReconciliation(requestCounter);
        }
    }


    // this is where most of the magic happens, it's hard to explain
    [Client]
    private void ClientReconciliation(int requestCounter){
        
        ClientRemovePastInputs(requestCounter);
            
        // at this stage, you have a list of every input since the last one confirmed by the server
        // now you want to loop over every past input and simulate physics using it
        Physics.autoSimulation = false;

        for (int i = 0; i < _pastInputs.Count; i++){
            PlayerInput playerInput = _pastInputs[i];
            if (playerInput.SprintInput){
                _isCrouching = false;
            }
            Move(playerInput.HorizontalInput);

            ClientRunKeyPresses(playerInput);
            
            Physics.Simulate(Time.fixedDeltaTime);

        }
        
        Physics.autoSimulation = true;
    }

    [Client]
    private void ClientRemovePastInputs(int requestCounter){
        List<PlayerInput> inputsToRemove = new List<PlayerInput>();

        // figure out what the server knows and what needs to be resimulated

        for (int i = 0; i < _pastInputs.Count; i++){
            if (_pastInputs[i].RequestNumber <= requestCounter){
                inputsToRemove.Add(_pastInputs[i]);
            }
            else{
                break;
            }
        }

        foreach (PlayerInput toRemove in inputsToRemove){
            _pastInputs.Remove(toRemove);
        }
    }

    [Client]
    private void ClientRunKeyPresses(PlayerInput playerInput){ // part of reconciliation
        if (playerInput.JumpInput){
            Jump();
        }
        


        if (playerInput.ReloadInput){
            primaryWeapon.Reload();
        }

        if (playerInput.AbilityInput){
            ArmorAbilityInstant();
        }

        if (playerInput.ModInput){
            ModAbilityInstant();
        }
    }
    
   
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
        public bool SprintInput;
        
        public bool AbilityInput; // for one time press
        public bool ModInput;

        public bool AbilityPressed; // for continuous press
        public bool ModPressed;
        
        public bool FiringInput;
        public float FiringAngle;
        public bool ReloadInput;

        public bool SwapWeapon;

        public BasicWeapon PickedUp; // new weapon trying to pick up
        public BasicWeapon OldWeapon; // if for some reason it fails need to go back to old one
        
        public int RequestNumber;
    }
}
