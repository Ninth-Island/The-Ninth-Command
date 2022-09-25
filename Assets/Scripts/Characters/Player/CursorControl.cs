
using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class CursorControl : MonoBehaviour{
    
    [SerializeField] private Texture2D[] Images;

    private Camera _mainCam;
    private Player _player;
    private CinemachineVirtualCamera _vc;
    

    private void Awake(){
        _mainCam = Camera.main;
        _vc = transform.parent.transform.GetChild(3).GetComponent<CinemachineVirtualCamera>();

        _player = transform.parent.GetComponent<Player>();



    }

    public void CameraFollow(float multiplier, float zoomRange){
        Vector2 playerPos = _player.transform.position;
        zoomRange *= multiplier;
        float x = Mathf.Clamp(GetMousePosition().x, playerPos.x - zoomRange, playerPos.x + zoomRange);
        float y = Mathf.Clamp(GetMousePosition().y, playerPos.y - zoomRange, playerPos.y + zoomRange);
        Vector2 pos = new Vector2(x, y);
        transform.position = pos;
        _vc.Follow = transform;

    }

    public void ResetCamera(){
        _vc.Follow = _player.transform;
        _vc.transform.position = _player.transform.position;
    }

    public void SetCursorType(int type){ ;
        Cursor.SetCursor(Images[type], new Vector2(32, 32), CursorMode.ForceSoftware);
    }
    public Vector3 GetMousePosition(){
        return _mainCam.ScreenToWorldPoint(Input.mousePosition);
    }


}
