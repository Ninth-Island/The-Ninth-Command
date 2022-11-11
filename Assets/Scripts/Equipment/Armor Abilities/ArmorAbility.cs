using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class ArmorAbility : Equipment{
    
    [Header("Armor Ability")]
    public int abilityIndex;
    public int maxCharge;
    public int chargeRate;
    public int chargeDrainPerFrame;
    [SerializeField] private SpriteRenderer previewIcon;

    [HideInInspector] public int currentAbilityCharge = 0;
    [HideInInspector] public bool active;
    
    protected override void Pickup(Player player, int[] path){
        base.Pickup(player, path);
        spriteRenderer.enabled = false;
        previewIcon.enabled = false;
        player.abilityImage.sprite = previewIcon.sprite;
        player.armorAbility = this;
    }

    public override void Drop(){
        base.Drop();
        spriteRenderer.enabled = true;
        previewIcon.enabled = true;

    }

    public virtual void ArmorAbilityInstant(float angle){ // called as soon as button pressed. In most cases checks if enough charge and then sets active to true
        
    }

    protected virtual void AbilityActiveFixedUpdate(){ // if active is true, do this every frame
        
    }

    public virtual void ArmorAbilityReleased(){ // called on button release. Only really used for jetpack rn
        
    }

    // this is called by clients only AND on server. This prevents host from running the logic twice and allows the clients to simulate too. 
    public virtual void ArmorAbilityFixedUpdate(){
        if (active){ // set active by instant if enough charge
            AbilityActiveFixedUpdate();
            
            currentAbilityCharge -= chargeDrainPerFrame;
            if (currentAbilityCharge <= 0){ // once used all charge turn it off to start recharging
                active = false;
            }
        }
        else{
            currentAbilityCharge = Mathf.Clamp(currentAbilityCharge + chargeRate, 0, maxCharge);
        }
    }


    [Server]
    public IEnumerator ServerInitializeArmorAbility(Player player, int[] path){
        wielder = player;
        netIdentity.AssignClientAuthority(player.connectionToClient);
        ClientSetWielder(player);

        yield return new WaitUntil(() => player.characterClientReady);
        CancelPickup(player, path);
        ClientInitializeEquipment(false, player, path);
    }

    protected override void ClientInitializeEquipment(bool isThePrimaryWeapon, Player player, int[] path){
        base.ClientInitializeEquipment(isThePrimaryWeapon, player, path);
        spriteRenderer.enabled = false;
    }

    protected void PlaySound(int index){
        if (hasAuthority){
            AudioManager.PlaySound(index);
        }
        if (isServer){
            PlaySoundClientRpc(index);
        }
    }

    private void PlaySoundClientRpc(int index){
        if (!hasAuthority){
            AudioManager.PlaySound(index);
        }
    }

    [Server]
    public void HideOnStart(){
        HideOnStartClientRpc();
    }

    [ClientRpc]
    private void HideOnStartClientRpc(){
        spriteRenderer.enabled = false;
        previewIcon.enabled = false;
    }
}
