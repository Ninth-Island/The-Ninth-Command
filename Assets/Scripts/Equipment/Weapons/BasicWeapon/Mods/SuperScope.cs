
using System;
using UnityEngine;

public class SuperScope : WeaponMod{
    
    // all this one does is turn a laser sight on and off. It also overrides primary weapon's scopes values

    [SerializeField] private int zoomIncrementsOverride;
    [SerializeField] private int totalZoomOverride;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private Transform endpoint;

    private Camera _mainCam;
    
    protected override void Start(){
        base.Start();
        WeaponAttachedTo.zoomIncrements = zoomIncrementsOverride;
        WeaponAttachedTo.totalZoom = totalZoomOverride;
        _mainCam = Camera.main;
        _lineRenderer.useWorldSpace = true;
        _lineRenderer.enabled = false;
    }

    protected override void OverrideInstant(){
        _lineRenderer.enabled = !_lineRenderer.enabled;
    }

    protected override void Update(){
        base.Update();
        if (!WeaponAttachedTo.activelyWielded){
            _lineRenderer.enabled = false;
        }
        else{
            RaycastHit2D hit = Physics2D.Raycast(WeaponAttachedTo.firingPoint.transform.position, WeaponAttachedTo.firingPoint.transform.right, 1000, LayerMask.GetMask("Ground", "Team 1", "Team 2", "Team 3", "Team 4"));
        
            _lineRenderer.SetPosition(0, WeaponAttachedTo.firingPoint.transform.position);
            if (hit){
                _lineRenderer.SetPosition(1, hit.point); 
            }
            else{
                _lineRenderer.SetPosition(1, endpoint.position);
            }
        }
    }

    public override void WeaponModFixedUpdate(){
        
    }
}
