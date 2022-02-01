
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


        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        float fov = 90;

        Vector3 origin = Vector3.zero;
        
        int rayCount = 50;
        float angle = 0f;
        float angleIncrease = fov / rayCount;
        float viewDistance = 50f;
        
        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        
        vertices[0] = origin;
        
        float angleRad = angle * (Mathf.PI / 180f);
        
        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i < rayCount; i++){
            Vector3 vertex = origin + new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * viewDistance;
            vertices[vertexIndex] = vertex;

            if (i > 0){
                triangles[triangleIndex + 0] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;

                triangleIndex += 3;
            }
            
            vertexIndex++;
            angle -= angleIncrease;
        }
        

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    public void CameraFollow(int zoomLevel, float zoomRange){
        Vector2 playerPos = _player.transform.position;
        zoomRange *= zoomLevel;
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
