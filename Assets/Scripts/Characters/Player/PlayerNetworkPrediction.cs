
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
            armorAbility.ArmorAbilityInstant(playerInput.Angle);
        }

        if (playerInput.ModInput){
            primaryWeapon.weaponMod.WeaponModInstant();
        }

        if (playerInput.SwapWeapon){
            PlayerSwapWeapon();
            PlayerSwapWeaponClientRpc();
        }

        if (playerInput.PickedUp){
            if (Vector2.Distance(playerInput.PickedUp.transform.position, transform.position) < 14 && playerInput.PickedUp.gameObject.layer == LayerMask.NameToLayer("Objects")){
                
                playerInput.PickedUp.netIdentity.AssignClientAuthority(connectionToClient);
                playerInput.OldEquipment.netIdentity.RemoveClientAuthority();
                int[] path = {0};
                if (playerInput.PickUpType == 0){
                    path = new[]{1, 3};
                }
                playerInput.PickedUp.SwapTo(this, playerInput.OldEquipment, path);

                PlayerPickUpEquipmentClientRpc(playerInput.PickedUp, playerInput.OldEquipment, playerInput.PickUpType);
            }
            else{
                playerInput.OldEquipment.CancelPickup(this, new[]{1, 3});
            }
        
        }
    }
    
    [ClientRpc]
    private void PlayerPickUpEquipmentClientRpc(Equipment newEquipment, Equipment oldEquipment, int pickedUpType){
        if (!hasAuthority){
            int[] path = {0};
            
            if (pickedUpType == 0){
                primaryWeapon.StopReloading();
                FinishReload();
                SetArmType(((BasicWeapon) newEquipment).armType);

                path = new[]{1, 3};
            }
            newEquipment.SwapTo(this, oldEquipment, path);
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
        SetClientValuesRpc(transform.position, transform.rotation, transform.localScale, _lastInput.ArmRotationInput, body.velocity, armorAbility.currentAbilityCharge, _lastInput.RequestNumber);
    }

    
    // the clients receive the information and update themselves accordingly
    [ClientRpc]
    private void SetClientValuesRpc(Vector3 position, Quaternion rotation, Vector3 scale, float armRotation, Vector2 velocity, int currentAbilityCharge, int requestCounter){

        // if its too far away (simulation lost track then dead reckon it
        // the simulation breaks down at high velocities which occasionally occur
        if (Vector3.Distance(transform.position, position) > 2 + 0.32f * body.velocity.magnitude){
            transform.position = position;
        }

        transform.rotation = rotation;
        RotateArm(armRotation); // fancier way of doing scale
        body.velocity = velocity;
        armorAbility.currentAbilityCharge = currentAbilityCharge;

       
        // if not too far away then reconcile with server by remembering all previous inputs and simulating them from server
        ClientReconciliation(requestCounter);
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
            armorAbility.ArmorAbilityInstant(playerInput.Angle);
        }

        if (playerInput.ModInput){
            primaryWeapon.weaponMod.WeaponModInstant();
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
        public float Angle;

        public bool AbilityPressed; // for continuous press
        public bool ModPressed;
        
        public bool FiringInput;
        public float FiringAngle;
        public bool ReloadInput;

        public bool SwapWeapon;
        

        public Equipment PickedUp; // new weapon trying to pick up
        public Equipment OldEquipment; // if for some reason it fails need to go back to old one
        public int PickUpType;
        
        public int RequestNumber;
    }
}
