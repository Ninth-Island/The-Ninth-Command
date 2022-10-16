
using System.Collections;
using System.Collections.Generic;
using Mirror;
using Pathfinding;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class NPC : Character{

    [SerializeField] private float sightRange = 40;
    [SerializeField] public float attackRange = 30;
    [SerializeField] private bool isPatrolling;

    private BoxCollider2D GroundChecker;

    private GameObject pivotPoint;
    public BasicWeapon _weapon;

    
    private GameObject healthBG;
    private GameObject healthFill;

    private Coroutine HealthRoutine;

    private Transform target = null;
    

    public override void OnStartClient(){
        base.OnStartClient();
        pivotPoint = transform.GetChild(1).GetChild(0).gameObject;
        _weapon = pivotPoint.transform.GetChild(3).GetComponent<BasicWeapon>();

        GroundChecker = transform.GetChild(4).GetComponent<BoxCollider2D>();

        healthBG = transform.GetChild(2).gameObject;
        healthFill = transform.GetChild(3).gameObject;

        InvokeRepeating(nameof(ScanForTargets), Random.Range(0, 10), 1f);
    }

    protected override void Update(){
        base.Update();
        UpdateHealthBar();
    }
    

    protected override void FixedUpdate(){
        base.FixedUpdate();
        if (target != null){
            
        }
        else{
            Patrol();
        }
    }

    [Server]
    public override void Hit(int damage){
        base.Hit(damage);

        if (HealthRoutine != null){
            StopCoroutine(HealthRoutine);
        }

        HealthRoutine = StartCoroutine(ShowHealthBar());
    }

    private void UpdateHealthBar(){

        Vector3 scale = healthFill.transform.localScale;
        Vector3 pos = healthFill.transform.localPosition;

        float newScale = (float) health / MaxHealth;
        float newPos = 2 - newScale * 2;
        if (transform.localScale.x > 0){
            newPos *= -1;
        }


        healthFill.transform.localScale = new Vector3(newScale * 4, scale.y, scale.x);
        healthFill.transform.localPosition = new Vector3(newPos, pos.y, pos.z);
    }

    private IEnumerator ShowHealthBar(){

        healthBG.SetActive(true);
        healthFill.SetActive(true);

        UpdateHealthBar();

        yield return new WaitForSeconds(3);

        healthBG.SetActive(false);
        healthFill.SetActive(false);
    }

    #region Patrolling

    private void Patrol(){
        if (isPatrolling){
            WallCheck();
            GroundCheck();
            body.velocity = new Vector2(moveSpeed * transform.localScale.x, body.velocity.y);
        }
    }

    private void ScanForTargets(){
        int mask = LayerMask.GetMask("Team 1", "Team 2", "Team 3", "Team 4") &~ (1<<gameObject.layer);

        RaycastHit2D[] raycastHit = new RaycastHit2D[1];
        Physics2D.CircleCastNonAlloc(transform.position, sightRange, new Vector2(0, 0), raycastHit, 0.1f, mask);

        RaycastHit2D hit = raycastHit[0];
        target = null;
        if (hit && IsInLineOfSight(hit.transform.position, mask)){
            target = hit.transform;
            target.position = hit.transform.position;
            
            AttackTarget();
        }
        
    }

    private bool IsInLineOfSight(Vector2 targetPos, int mask){
        int newMask = mask | (1 << LayerMask.NameToLayer("Ground"));
        Vector2 direction = new Vector2(targetPos.x - transform.position.x, targetPos.y - transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, sightRange, newMask);
        
        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground")){
            return false;
        }
        return true;
    }

    private void AttackTarget(){
        if (Vector2.Distance(target.position, transform.position) > attackRange){
            transform.localScale = new Vector3(Mathf.Sign(target.position.x - transform.position.x), 1, 1);
            body.velocity = new Vector2(moveSpeed * transform.localScale.x, body.velocity.y);
        }
        else{
            _weapon.CmdAttemptFire(GetNpcToTargetAngle() * Mathf.Deg2Rad);
        }
    }

    private float GetNpcToTargetAngle(){
        return Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg;

    }

    private void WallCheck(){
        RaycastHit2D raycastHit = Physics2D.CircleCast(transform.position, 2, Vector2.right * transform.localScale.x, 1,
            LayerMask.GetMask("Ground", "Vehicle Outer"));

        if (raycastHit.collider){
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y);
        }
    }

    private void GroundCheck(){
        if (!Physics2D.IsTouchingLayers(GroundChecker, LayerMask.GetMask("Ground")) && !Airborne){
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y);

        }
    }


    #endregion


    
    
}
    
