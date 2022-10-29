using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModeManager : NetworkBehaviour{
    
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

    [SerializeField] private float timeLeft;

    [SerializeField] private int maxScore;
    private int _bluePoints;
    private Slider _blueSlider;
    private TextMeshProUGUI _blueText;
    
    private int _redPoints;
    private Slider _redSlider;
    private TextMeshProUGUI _redText;

    private TextMeshProUGUI _timerText;
    
    
    private void Start(){
        TeamsSpawns[] teamsSpawnsArray = FindObjectsOfType<TeamsSpawns>();
        _blueSpawns = teamsSpawnsArray[0].GetSpawns();
        _redSpawns = teamsSpawnsArray[1].GetSpawns();

        _blueSlider = transform.GetChild(0).transform.GetChild(0).GetComponent<Slider>();
        _blueText = _blueSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        _redSlider = transform.GetChild(0).transform.GetChild(2).GetComponent<Slider>();
        _redText = _redSlider.transform.GetChild(3).GetComponent<TextMeshProUGUI>();

        _timerText = transform.GetChild(0).transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
    }
    public void Die(Player player){
        if (gameMode == 0){
            if (player.teamIndex < 7){
                _bluePoints++;
            }
            else{
                _redPoints++;
            }
            UpdateScoreClientRpc(_bluePoints, _redPoints);
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

    [Server]
    public void AllPlayersReady(){
        UpdateScoreClientRpc(_bluePoints, _redPoints);
    }

    [ClientRpc]
    private void UpdateScoreClientRpc(int bluePoints, int redPoints){
        _blueSlider.value = (float) bluePoints / maxScore;
        _blueText.text = $"{bluePoints}/{maxScore}";

        _redSlider.value = (float) redPoints / maxScore;
        _redText.text = $"{redPoints}/{maxScore}";
    }

    private void Update(){
        timeLeft -= Time.deltaTime;
        int minutes = Mathf.RoundToInt(timeLeft) / 60;
        _timerText.text = $"{minutes}:{Mathf.RoundToInt(timeLeft) - minutes * 60}";
    }
}