
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
            //CheckZoom();
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

                float sign = Mathf.Sign(wheelInput);

                _multiplier = Mathf.Clamp(_multiplier + (sign * _incrementSize / zoomRange), 0, 1);
                
                if (sign > 0){
                    AudioManager.PlaySound(0);
                }
                else if (sign < 0){
                    AudioManager.PlaySound(1);
                }
            }
            
            _cursorControl.CameraFollow(_multiplier, zoomRange);
        }
    }
    
}
