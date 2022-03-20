
using System.Collections;
using Pathfinding;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class NPC : Character{

    [SerializeField] private bool isPatrolling;

    private BoxCollider2D GroundChecker;
    
    private GameObject healthBG;
    private GameObject healthFill;

    private Coroutine HealthRoutine;



    protected override void Start(){
        base.Start();

        GroundChecker = transform.GetChild(4).GetComponent<BoxCollider2D>();

        healthBG = transform.GetChild(2).gameObject;
        healthFill = transform.GetChild(3).gameObject;

    }

    protected override void Update(){
        base.Update();
        UpdateHealthBar();
    }
    

    protected override void FixedUpdate(){
        base.FixedUpdate();
        Patrol();

    }

    protected override void TakeDamage(int damage){
        base.TakeDamage(damage);

        if (HealthRoutine != null){
            StopCoroutine(HealthRoutine);
        }

        HealthRoutine = StartCoroutine(ShowHealthBar());
    }

    private void UpdateHealthBar(){

        Vector3 scale = healthFill.transform.localScale;
        Vector3 pos = healthFill.transform.localPosition;

        float newScale = (float) health / maxhealth;
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
            Body.velocity = new Vector2(moveSpeed * transform.localScale.x, Body.velocity.y);
        }
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
    
