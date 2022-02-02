using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Mesh mesh;
    private float fov;
    private float viewDistance;
    private Vector3 origin;
    private float startingAngle;

    private Player _player;

    private void Start() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        fov = 90f;
        viewDistance = 50f;
        origin = Vector3.zero;
        _player = FindObjectOfType<Player>();
    }

    private void LateUpdate() {
        
        SetOrigin();
        
        int rayCount = 50;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for (int i = 0; i <= rayCount; i++) {
            Vector3 vertex;
            RaycastHit2D raycastHit2D = Physics2D.Raycast(origin, GetVectorFromAngle(angle), viewDistance, layerMask);
            if (raycastHit2D.collider == null) {
                // No hit
                vertex = origin + GetVectorFromAngle(angle) * viewDistance;
            } else {
                // Hit object
                vertex = raycastHit2D.point;
                Debug.Log(raycastHit2D.collider.gameObject);
            }
            vertices[vertexIndex] = vertex;

            if (i > 0) {
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
        mesh.bounds = new Bounds(origin, Vector3.one * 1000f);
    }

    public void SetOrigin() {
        this.origin = _player.Helmet.position;
        startingAngle = _player.GetPlayerToMouseRotation() + fov / 2f; 
    }


    public void SetFoV(float fov) {
        this.fov = fov;
    }

    public void SetViewDistance(float viewDistance) {
        this.viewDistance = viewDistance;
    }
    private Vector3 GetVectorFromAngle(float angle) {
        // angle = 0 -> 360
        float angleRad = angle * (Mathf.PI/180f);
        return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
    }
    
}
