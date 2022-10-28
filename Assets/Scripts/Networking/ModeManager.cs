using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : MonoBehaviour{
    [SerializeField] private int gameMode; 
    /* 0 = slayer
     * 1 = capture the flag
     * 2 = capture the zone
     * 3 = siege (destroy enemy base)
     * 4 = deliver the explosive
     */
    private List<Vector3> _blueSpawns = new List<Vector3>();
    private int _blueSpawnCounter;
    private List<Vector3> _redSpawns = new List<Vector3>();
    private int _redSpawnCounter;
    
    private int _redPoints;
    private int _bluePoints;
    private void Start(){
        TeamsSpawns[] teamsSpawnsArray = FindObjectsOfType<TeamsSpawns>();
        _blueSpawns = teamsSpawnsArray[0].GetSpawns();
        _redSpawns = teamsSpawnsArray[1].GetSpawns();
    }
    public void Die(Player player){
        if (gameMode == 0){
            if (player.teamIndex < 7){
                _bluePoints++;
            }
            else{
                _redPoints++;
            }
            
            Respawn(player);
        }
    }
    private void Respawn(Player player){
        if (player.teamIndex > 6){
            player.virtualPlayer.Respawn(_redSpawns[_redSpawnCounter]);
            _redSpawnCounter++;
            if (_redSpawnCounter >= _redSpawns.Count){
                _redSpawnCounter = 0;
            }
        }
        
        else{
            player.virtualPlayer.Respawn(_blueSpawns[_blueSpawnCounter]);
            _blueSpawnCounter++;
            if (_blueSpawnCounter >= _blueSpawns.Count){
                _blueSpawnCounter = 0;
            }
        }
    }
}