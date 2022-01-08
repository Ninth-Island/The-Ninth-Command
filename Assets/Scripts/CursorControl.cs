
using System;
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class CursorControl : MonoBehaviour{
    
    [SerializeField] private Texture2D[] Images;

    
    private Camera mainCam;
    

    private void Start(){
        mainCam = Camera.main;
    }

    public void SetCursorType(int type){ ;
        Cursor.SetCursor(Images[type], new Vector2(32, 32), CursorMode.ForceSoftware);
    }
    public Vector2 GetMousePosition(){
        return mainCam.ScreenToWorldPoint(Input.mousePosition);
    }
}
