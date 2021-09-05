using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class WeaponPickup : PlayerControl{
    
    public Dictionary<GameObject, KeyValuePair<BasicWeapon, Rigidbody2D>> _allBasicWeapons = new Dictionary<GameObject, KeyValuePair<BasicWeapon, Rigidbody2D>>();
    protected override void Update(){
        base.Update();
        CheckUpdates();
    }

    protected override void Start(){
        base.Start();
    }

    private void CheckUpdates(){
        Vector2 mousePos = GetMousePosition();
        RaycastHit2D weaponScan = Physics2D.Linecast(transform.position, mousePos, LayerMask.GetMask("Objects"));

        if (weaponScan && weaponScan.collider.gameObject.CompareTag("Weapon")){
            var nearestWeapon = weaponScan.collider.gameObject;
            pickupText.transform.localPosition = Input.mousePosition - HUD.transform.localPosition;
            pickupText.SetText(nearestWeapon.name);
            BasicWeapon weapon = _allBasicWeapons[nearestWeapon].Key;
            
            if (player.IsTouching(transform.position, weaponScan.collider.gameObject.transform.position, weapon.GetPickupRange(), weapon.GetPickupRange())){
                weapon.PickUp(player);
            }
        }
    }

    public void AddWeapon(KeyValuePair<GameObject, KeyValuePair<BasicWeapon, Rigidbody2D>> weapon){
        _allBasicWeapons.Add(weapon.Key, weapon.Value);
    }

}
