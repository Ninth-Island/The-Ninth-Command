using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class Flip : MonoBehaviour{

    public AIPath aiPath;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = new Vector3(1, 1, 1);
        if (aiPath.desiredVelocity.x < 0){
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
