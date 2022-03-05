using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using UnityEngine;

public class Scope : ActiveMod{
    
    [SerializeField] private int zoomIncrements;
    [SerializeField] private float zoomRange;

    private CursorControl _cursorControl;
    
    private float _multiplier = 1;
    private float _incrementSize;

    protected override void Start(){
        base.Start();

        _cursorControl = FindObjectOfType<CursorControl>();
        
        _incrementSize = zoomRange / zoomIncrements;
        _multiplier = _incrementSize / zoomRange;
    }

    protected override void Update(){
        base.Update();
        
        if (IsReady){
            base.Update();
            CheckZoom();
        }
    }
    
    private void CheckZoom(){
        _cursorControl.ResetCamera();
        if (Input.GetKeyUp(KeyCode.Mouse1)){
            _multiplier = _incrementSize / zoomRange;
        }
        if (Input.GetKey(KeyCode.Mouse1)){

            float wheelInput = Input.GetAxis("Mouse ScrollWheel");
            if (zoomIncrements > 0 && Input.GetKey(KeyCode.Mouse1) && wheelInput != 0){
                
                _multiplier = Mathf.Clamp(_multiplier + (Mathf.Sign(wheelInput) * _incrementSize / zoomRange), 0, 1);
            }
            
            _cursorControl.CameraFollow(_multiplier, zoomRange);
        }
    }
    
}
