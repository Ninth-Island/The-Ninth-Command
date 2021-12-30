
using UnityEngine;
using UnityEngine.UI;
using Vector3 = UnityEngine.Vector3;

public class CursorControl : MonoBehaviour{
    
    [SerializeField] private Sprite[] Sprites;

    private SpriteRenderer imageRenderer;
    
    private Camera mainCam;

    private void Start(){/*
        Cursor.visible = false;*/
        mainCam = Camera.main;
        imageRenderer = GetComponent<SpriteRenderer>();
        imageRenderer.sprite = Sprites[0];
    }

    private void Update(){
        Vector3 pos = transform.position;
        pos = mainCam.ScreenToWorldPoint(Input.mousePosition);
        pos = new Vector3(pos.x, pos.y, 0);
        transform.position = pos;
    }

    public void SetCursorType(int type){
        imageRenderer.sprite = Sprites[type];
    }
}
