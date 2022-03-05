
using System;
using Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class CursorControl : MonoBehaviour{
    
    [SerializeField] private Texture2D[] Images;

    
    private Camera mainCam;
    private Player _player;
    private CinemachineVirtualCamera vc;




    private void Start(){
        mainCam = Camera.main;
        vc = mainCam.transform.parent.transform.GetChild(2).transform.GetChild(0)
            .GetComponent<CinemachineVirtualCamera>();
        _player = FindObjectOfType<Player>();



    }

    public void CameraFollow(float multiplier, float zoomRange){
        Vector2 playerPos = _player.transform.position;
        zoomRange *= multiplier;
        float x = Mathf.Clamp(GetMousePosition().x, playerPos.x - zoomRange, playerPos.x + zoomRange);
        float y = Mathf.Clamp(GetMousePosition().y, playerPos.y - zoomRange, playerPos.y + zoomRange);
        Vector2 pos = new Vector2(x, y);
        transform.position = pos;
        vc.Follow = transform;

    }

    public void ResetCamera(){
        vc.Follow = _player.transform;
        vc.transform.position = _player.transform.position;
    }

    public void SetCursorType(int type){ ;
        Cursor.SetCursor(Images[type], new Vector2(32, 32), CursorMode.ForceSoftware);
    }
    public Vector2 GetMousePosition(){
        return mainCam.ScreenToWorldPoint(Input.mousePosition);
    }


}
