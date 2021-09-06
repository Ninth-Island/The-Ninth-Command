using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Player : Character{
    
    /*
* ================================================================================================================
*                                               Player
*
 * contains all player logic. Movement, weapons, swapping weapons, animation, sounds, and additional foot collider
*
* 
* ================================================================================================================
*/

    [SerializeField] public BasicWeapon primaryWeapon;
    [SerializeField] public BasicWeapon secondaryWeapon;

    
    [SerializeField] private float jumpPower;
    [SerializeField] private float jetPower;

    private BoxCollider2D _feetCollider;


    private PlayerControl _playerControl;

    
    

    private string running = "Running";
    private string runningBackwards = "RunningBackwards";
    private string jumping = "Jumping";
    //private string falling = "Falling";
    /*private string kicking = "Kicking";
    private string punching = "Punching";
    private string comboing = "Comboing";
    private string dead = "Dead";*/
    
    protected override void Start(){
        base.Start();

        DeathAnimationName = "Dead";
        _playerControl = GetComponent<PlayerControl>();
        _feetCollider = transform.GetChild(2).GetComponent<BoxCollider2D>();
        
    }

    protected override void FixedUpdate(){
        base.FixedUpdate();
        Move();
        CheckJetpack();
    }

    protected override void Update(){
        base.Update();
        Jump();
        CheckSwap();
        primaryWeapon.CheckReload();
    }

    /*
    * ================================================================================================================
    *                                               Movement
    * ================================================================================================================
    */

    private void Move(){
        float input = Input.GetAxis("Horizontal");
        Animator.SetBool(running, input != 0);
        if (input != 0 && !InputsFrozen && !Knocked){
            Body.velocity = new Vector2(moveSpeed * input, Body.velocity.y);
            Animator.SetBool(runningBackwards, Math.Sign(input) != Math.Sign(transform.localScale.x));
        }
     
    }

    private void Jump(){
        Animator.SetBool(jumping, Airborne);
        
        if (Input.GetKey(KeyCode.W)){
            Vector2 velocity = Body.velocity;
            
            if (!Airborne){
                Airborne = true;
                Animator.SetBool(jumping, true);
                Body.velocity = new Vector2(velocity.x, jumpPower);
                AudioManager.PlayFromList(1);

            }
        }
    }

    private void CheckJetpack(){
        Airborne = true;
        RaycastHit2D clampScan = Physics2D.Raycast(transform.position, Vector2.down, 4,
            LayerMask.GetMask("Ground", "Platform"));
        
        if (clampScan.collider){
            Airborne = false;
            
        }
        
        if (Input.GetKey(KeyCode.W) && Airborne){
            Body.AddForce(Vector2.up * jetPower, ForceMode2D.Impulse);
        }
    }
    
    private void Transform(float x){
            Vector2 pos = transform.position;
            transform.position = new Vector3(pos.x + x * transform.localScale.x, pos.y);
        }

    
    /*
    * ================================================================================================================
    *                                               Firing/Attacking
    * ================================================================================================================
    */
   

    private void CheckSwap(){
        if (Input.GetKeyDown(KeyCode.Mouse1)){
            primaryWeapon.SetSpriteRenderer(false);
            secondaryWeapon.SetSpriteRenderer(true);

            primaryWeapon.SetLoadingState();

            BasicWeapon temp = primaryWeapon;
            primaryWeapon = secondaryWeapon;
            secondaryWeapon = temp;
            
            primaryWeapon.RefreshText();
        }
    }
    
    /*
    * ================================================================================================================
    *                                               Other
    * ================================================================================================================
    */


    public bool IsTouching(Vector2 pos1, Vector2 pos2, float xAffordance, float yAffordance){
        if (Math.Abs(pos1.x - pos2.x) < xAffordance && Math.Abs(pos1.y - pos2.y) < yAffordance){
            return true;
        }
        return false;
    }
    
    protected override void TakeDamage(int damage){
        base.TakeDamage(damage);
    }


    public BoxCollider2D GetFeetCollider(){
        return _feetCollider;
    }
    
}
