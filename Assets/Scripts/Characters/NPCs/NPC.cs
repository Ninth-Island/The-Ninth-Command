
using System.Collections;
using Pathfinding;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class NPC : Character{

    public Transform target;
    public float nextWayPointDistance = 3;
    private Path _path;
    private int _currentWaypoint;
    private bool _reachedEndOfPath = false;

    private Seeker _seeker;

    private BoxCollider2D wallDetector;
    private BoxCollider2D groundDetector;

    private GameObject healthBG;
    private GameObject healthFill;

    private Coroutine HealthRoutine;
    


    protected override void Start(){
        base.Start();

        _seeker = GetComponent<Seeker>();
        InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
        
        // remove later
        target = FindObjectOfType<Player>().transform;
        
        wallDetector = transform.GetChild(2).GetComponent<BoxCollider2D>();
        groundDetector = transform.GetChild(3).GetComponent<BoxCollider2D>();

        healthBG = transform.GetChild(4).gameObject;
        healthFill = transform.GetChild(5).gameObject;

    }

    protected override void Update(){
        base.Update();
        
    }
    
    protected override void FixedUpdate(){
        base.FixedUpdate();


        Navigate();
    }
    
    protected override void TakeDamage(int damage){
        base.TakeDamage(damage);
        
        if (HealthRoutine != null){
            StopCoroutine(HealthRoutine);
        }
        
        HealthRoutine = StartCoroutine(ShowHealthBar());
    }

    private IEnumerator ShowHealthBar(){
        healthBG.SetActive(true);
        healthFill.SetActive(true);

        Vector3 scale = healthFill.transform.localScale;
        Vector3 pos = healthFill.transform.localPosition;
        
        float newScale = (float) health / maxhealth;
        float newPos = 2 - newScale * 2;
        
        healthFill.transform.localScale = new Vector3(newScale * 4, scale.y, scale.x);
        healthFill.transform.localPosition = new Vector3(newPos, pos.y, pos.z);
        

        yield return new WaitForSeconds(3);

        healthBG.SetActive(false);
        healthFill.SetActive(false);
    }


    #region Pathfinding

    
    private void Navigate(){
        if (_path == null){
            return;
        }

        if (_currentWaypoint >= _path.vectorPath.Count){
            _reachedEndOfPath = true;
            return;
        }
        else{
            _reachedEndOfPath = false;
        }

        Vector2 direction = (_path.vectorPath[_currentWaypoint] - transform.position).normalized;
        Vector2 force = direction * moveSpeed * Time.deltaTime;
        
        Move(force);

        float distance = Vector2.Distance(transform.position, _path.vectorPath[_currentWaypoint]);
        if (distance < nextWayPointDistance){
            _currentWaypoint++;
        }
    }
    
    private void OnPathComplete(Path path){
        if (!path.error){
            _path = path;
            _currentWaypoint = 0;
        }
    }

    void UpdatePath(){
        if (_seeker.IsDone()){
            _seeker.StartPath(transform.position, target.position, OnPathComplete);
        }
    }
    
    private void Move(Vector2 force){
        float forceX = Mathf.Sign(force.x) * moveSpeed;
        float forceY = 0;

        RaycastHit2D raycastHit = Physics2D.Raycast(FeetCollider.transform.position,
            new Vector2(transform.localScale.x, 0), 3.5f, LayerMask.GetMask("Ground", "Vehicle Outer"));

        if (raycastHit.collider != null){
            Debug.Log(raycastHit.collider.gameObject);
            Debug.DrawRay(FeetCollider.transform.position,
                new Vector2(transform.localScale.x, 0) * 3.5f);
            forceY = jumpPower;
        }
        
        Body.velocity = new Vector2(forceX, forceY);
        transform.localScale = new Vector3(Mathf.Sign(forceX), 1);
    }

    #endregion

    
    
}
    
