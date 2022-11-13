
using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class CursorControl : MonoBehaviour{
    
    [SerializeField] private Texture2D[] Images;

    private Camera _mainCam;
    private Player _player;
    

    private void Awake(){
        _mainCam = Camera.main;

        _player = transform.parent.GetComponent<Player>();
    }

    public void CameraFollow(Transform t){
        _player.virtualCamera.Follow = t;
    }

    public void ResetCamera(){
        _player.virtualCamera.Follow = _player.transform;
    }

    public void SetCursorType(int type){ ;
        Cursor.SetCursor(Images[type], new Vector2(32, 32), CursorMode.ForceSoftware);
    }
    public Vector3 GetMousePosition(){
        return _mainCam.ScreenToWorldPoint(Input.mousePosition);
    }


}
