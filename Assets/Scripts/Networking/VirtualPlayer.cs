using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;

public class VirtualPlayer : NetworkBehaviour{

    private PlayerTag _playerTag;

    public void SetPlayerTag(PlayerTag playerTag){
        _playerTag = playerTag;
    }

    public PlayerTag GetPlayerTag(){
        return _playerTag;
    }
    
    
    
    
}
