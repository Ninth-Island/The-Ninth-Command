using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;

public class BasicWeapon : Weapon{
    
    /*
   * ================================================================================================================
   *                                  Basic Weapon --> Weapon
     *
     * Any weapon that can be held in the hands. Energy Sword, Gravity Hammer, Guns, Lasers. Primary and Secondary
     *
     * Contains logic for picking up and some virtuals called by player
     *
     * 
   * ================================================================================================================
   */
    


    protected Coroutine Coroutine;
    
    public bool looping;

    
    
    protected override void Start(){
        base.Start();
        PlayerPickupController.AddWeapon(new KeyValuePair<GameObject, KeyValuePair<BasicWeapon, Rigidbody2D>>(gameObject, new KeyValuePair<BasicWeapon, Rigidbody2D>(this, GetComponent<Rigidbody2D>())));
    }
    
    

    /*
     * ================================================================================================================
     *                                               Pickup and Flipping
     * ================================================================================================================
     */
    
    protected virtual void Update(){
        if (Player.primaryWeapon == this){
            Flip();
            if (Input.GetKey(KeyCode.Mouse0)){
                
                CheckFire();
            }
        }

    }

    public override void PickUp(Character pickedUpBy){
        PlayerPickupController.pickupText.SetText("(G) " + name);

        if (Input.GetKeyDown(KeyCode.G)){
            RefreshText();

            base.PickUp(pickedUpBy);
            Player.primaryWeapon.Drop();
            transform.localPosition = new Vector3(0.8f, 1);
            Player.primaryWeapon = this;
        }
    }

    protected override void Drop(){
        base.Drop();
        
    }
    
    
    private void Flip(){
        float rotation = PlayerPickupController.GetPlayerToMouseRotation();
        transform.rotation = Quaternion.Euler(0, 0, rotation);
        transform.localScale = new Vector3(1, 1);
        Player.transform.localScale = new Vector3(1, 1);
        if (rotation > 90 && rotation < 270){
            transform.localScale = new Vector3(-1, -1);
            Player.transform.localScale = new Vector3(-1, 1);
        }
    }
    
    /*
     * ================================================================================================================
     *                                               Virtuals
     * ================================================================================================================
     */
    
    
    protected virtual void Subtract(){
        AudioManager.PlayFromList(0);
    }

    
    protected virtual void CheckFire(){
        
    }
    public virtual void RefreshText(){
        PlayerPickupController.WeaponImage.sprite = SpriteRenderer.sprite;
    }
    
    /*
     * ================================================================================================================
     *                                               Other
     * ================================================================================================================
     */

    public virtual void CheckReload(){
        
    }

    public virtual void SetLoadingState(){
        
    }
    
    public void SetSpriteRenderer(bool setEnabled){
        SpriteRenderer.enabled = setEnabled;
    }
    
    
}
