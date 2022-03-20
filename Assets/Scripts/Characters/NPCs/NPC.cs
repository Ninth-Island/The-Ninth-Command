
using System.Collections;
using Pathfinding;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class NPC : Character{
    
    private GameObject healthBG;
    private GameObject healthFill;

    private Coroutine HealthRoutine;
    


    protected override void Start(){
        base.Start();
        
        healthBG = transform.GetChild(2).gameObject;
        healthFill = transform.GetChild(3).gameObject;

    }

    protected override void Update(){
        base.Update();
        UpdateHealthBar();
    }
    
    protected override void FixedUpdate(){
        base.FixedUpdate();

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




    
    
}
    
