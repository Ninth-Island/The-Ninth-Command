using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamsSpawns : MonoBehaviour{

    [SerializeField] private List<Transform> _spawns;

    public List<Vector3> GetSpawns(){
        List<Vector3> spawns = new List<Vector3>();
        foreach (Transform spawn in _spawns){
            spawns.Add(spawn.position);
        }

        return spawns;
    }

}
