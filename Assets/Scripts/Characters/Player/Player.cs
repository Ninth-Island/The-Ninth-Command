using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    [SerializeField] public BasicWeapon primaryWeapon;
    [SerializeField] public BasicWeapon secondaryWeapon;

    
    [SerializeField] private float jumpPower;
    [SerializeField] private float jetPower;

    [SerializeField] private GameObject spritesParent;

    [SerializeField] private Sprite[] ArmTypes;

    private Transform _arm;
    public Transform Helmet;

    private SpriteRenderer _armRenderer;



    private bool _isCrouching;

    
    


    private ANames _aNames = new ANames();
    private Color[] _colors = new Color[3];
    private GameObject[] sprites = new GameObject[7];

    private class ANames{
        public readonly string running = "Running";
        public readonly  string runningBackwards = "RunningBackward";
        public readonly  string crouching = "Crouching";
        public readonly  string jumping = "Jumping";
        public readonly  string punching = "Punch";
        public readonly  string dying = "Dying";
    }


    #region Start And Update

    /*
    * ================================================================================================================
    *                                               Start and update
    * ================================================================================================================
    */

    protected override void Start(){
        base.Start();


        _arm = transform.GetChild(1).transform.GetChild(5);
        Helmet = transform.GetChild(1).transform.GetChild(4);

        _armRenderer = _arm.GetChild(0).GetComponent<SpriteRenderer>();

        
        for (int i = 0; i < spritesParent.transform.childCount; i++){
            sprites[i] = spritesParent.transform.GetChild(i).gameObject;
        }
        
        Start2();
    }

    protected override void FixedUpdate(){
        base.FixedUpdate();
        Move();
        Jump();
    }

    protected override void Update(){
        base.Update();

        CheckSwap();
        
        CheckCrouch();
        primaryWeapon.CheckReload();
        RotateArm();
        
        
        
        Update2();
        CheckForPickup();
    }
    #endregion

    #region Movement
    /*
    * ================================================================================================================
    *                                               Movement
    * ================================================================================================================
    */

    private void Move(){
        float input = Input.GetAxis("Horizontal");
        Animator.SetBool(_aNames.running, input != 0);

        if (input != 0 && !InputsFrozen && !FallingKnocked){

            Animator.SetBool(_aNames.runningBackwards, Math.Sign(input) != Math.Sign(transform.localScale.x));
            
            if (_isCrouching){
                Body.velocity = new Vector2(moveSpeed / 2 * input, Body.velocity.y);
                SortSound(2);
            }
            else{
                Body.velocity = new Vector2(moveSpeed * input, Body.velocity.y);
                SortSound(1);
            }
            
        }
        
        //CheckJetpack();
    }

    private void Jump(){
        
        
        if (Input.GetKey(KeyCode.W)){
            Vector2 velocity = Body.velocity;
            
            // can't use airborne because the player is considered not airborne a few seconds before and after jumping
            if (FeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground", "Platform", "Vehicle", "Vehicle Outer"))){
                Airborne = true;
                Body.velocity = new Vector2(velocity.x, velocity.y + jumpPower);
                
                SortSound(0);
            }
        }
        
        Animator.SetBool(_aNames.jumping, Airborne);
    }

    private void CheckJetpack(){
        if (Input.GetKey(KeyCode.W) && Airborne){
            Body.AddForce(Vector2.up * jetPower, ForceMode2D.Impulse);
        }
    }
    
    

    private void CheckCrouch(){
        if (Input.GetKeyDown(KeyCode.S)){
            _isCrouching = !_isCrouching;
            Animator.SetBool(_aNames.crouching, _isCrouching);
        }
    }

    private void Transform(float x){ 
        Vector2 pos = transform.position;
        transform.position = new Vector3(pos.x + x * transform.localScale.x, pos.y);
    }
    
    #endregion

    #region Weapon and Vehicle
    /*
    * ================================================================================================================
    *                                               Weapon and Vehicle
    * ================================================================================================================
    */
    
    private void CheckSwap(){

        if (Math.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0 && !Input.GetKey(KeyCode.Mouse1)){
            if (primaryWeapon != null){
                primaryWeapon.SetSpriteRenderer(false);
            }

            if (secondaryWeapon != null){
                secondaryWeapon.SetSpriteRenderer(true);
            }

            primaryWeapon.SetLoadingState();

            (primaryWeapon, secondaryWeapon) = (secondaryWeapon, primaryWeapon);

            primaryWeapon.PickUp(this);
            SetArmType(primaryWeapon.armType);
        }
    }
    
    private void CheckForPickup(){
        Vector2 mousePos = _cursorControl.GetMousePosition();
        RaycastHit2D objectScan =
            Physics2D.CircleCast(mousePos, 1, new Vector2(), 0, LayerMask.GetMask("Objects", "Vehicle"));
        
        pickupText.SetText("");
        if (objectScan){
            GameObject nearestObject = objectScan.collider.gameObject;
            pickupText.transform.localPosition = Input.mousePosition - HUD.transform.localPosition;
            pickupText.SetText(nearestObject.name);

            if (nearestObject.CompareTag("Weapon")){
                WeaponPickup(nearestObject);
            }

            if (nearestObject.CompareTag("Vehicle")){
                VehicleEmbark(nearestObject);
            }
        }
    }

    private void WeaponPickup(GameObject nearestObject){
        BasicWeapon weapon = _allBasicWeapons[nearestObject].Key;

        if (IsTouching(transform.position, nearestObject.transform.position,
            weapon.GetPickupRange(), weapon.GetPickupRange())){
            pickupText.SetText("(G) " + weapon.name);
            if (Input.GetKeyDown(KeyCode.G)){
                primaryWeapon.Drop();
                
                primaryWeapon = weapon;
                weapon.PickUp(this);
            }
        }
    
    }

    private void VehicleEmbark(GameObject nearestObject){
        Vehicle vehicle = _allVehicles[nearestObject];
        if (IsTouching(transform.position, nearestObject.transform.position, vehicle.GetEmbarkRange(),
            vehicle.GetEmbarkRange())){
            pickupText.SetText("(G) " + nearestObject.name);
            if (Input.GetKeyDown(KeyCode.G)){
                SetNotifText("Embarked " + gameObject.name);
                gameObject.SetActive(false);

                vehicle.SetDriver(this);
                SpriteRenderer driver = vehicle.GetDriver();
                SpriteRenderer driverVisor = vehicle.GetDriverVisor();
                driver.enabled = true;
                driverVisor.enabled = true;

                Transform helmet = transform.GetChild(1).GetChild(5);
                driver.color = helmet.GetChild(0).GetComponent<SpriteRenderer>().color;
                driverVisor.color = helmet.GetChild(2).GetComponent<SpriteRenderer>().color;
                
                        
            }
        }
    }

    private void RotateArm(){
        float rotation = GetPlayerToMouseRotation();
        _arm.transform.rotation = Quaternion.Euler(0, 0, rotation);
        _arm.transform.localScale = new Vector3(1, 1);
        
        Helmet.transform.rotation = Quaternion.Euler(0, 0, rotation);
        Helmet.transform.localScale = new Vector3(1, 1);

        
        transform.localScale = new Vector3(1, 1);
        if (rotation > 90 && rotation < 270){
            _arm.transform.localScale = new Vector3(-1, -1);
            Helmet.transform.localScale = new Vector3(-1, -1);
            transform.localScale = new Vector3(-1, 1);
        }
    }

    public void SetArmType(int armType){
        _armRenderer.sprite = ArmTypes[armType];
    }
    
    public void AddWeapon(KeyValuePair<GameObject, KeyValuePair<BasicWeapon, Rigidbody2D>> weapon){
        _allBasicWeapons.Add(weapon.Key, weapon.Value);
    }
    public void AddVehicle(KeyValuePair<GameObject, Vehicle> vehicle){
        _allVehicles.Add(vehicle.Key, vehicle.Value);
    }
    
    public Dictionary<GameObject, KeyValuePair<BasicWeapon, Rigidbody2D>> _allBasicWeapons = new Dictionary<GameObject, KeyValuePair<BasicWeapon, Rigidbody2D>>();
    public Dictionary<GameObject, Vehicle> _allVehicles = new Dictionary<GameObject, Vehicle>();

    
    #endregion

    #region Sound
    /*
    * ================================================================================================================
    *                                               Sound
    * ================================================================================================================
    */
    
    
   
    #endregion
    
    #region Other
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
    
    
    public BoxCollider2D GetFeetCollider(){
        return FeetCollider;
    }
    

    protected override void TakeDamage(int damage){
        base.TakeDamage(damage);
    }

    #endregion
    


    

    
}
