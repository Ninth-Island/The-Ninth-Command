using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class WeaponPickup : CustomObject
{
    
    [SerializeField] protected BasicWeapon weapon;
    private AudioManager _audioManager;
    


    public virtual void PickUp(Character pickedUpBy, int[] path){
        CmdServerPickup(pickedUpBy, path);
        // AudioManager.PlayFromList(2);
    }

    [Command(requiresAuthority = false)]
    private void CmdServerPickup(Character wielder, int[] path){
        if (Vector2.Distance(transform.position, wielder.transform.position) < 10){
            ClientReceivePickup(wielder, path);
            gameObject.SetActive(false);
            Invoke(nameof(Despawn), 1f);
        }
    }

    [Server]
    private void Despawn(){
        NetworkServer.Destroy(gameObject);
        
    }

    [ClientRpc]
    private void ClientReceivePickup(Character wielder, int[] path){
        Transform currentChild = wielder.transform;
        for (int i = 0; i < path.Length; i++){
            currentChild = currentChild.GetChild(path[i]);
        }
        wielder.PickupWeapon(Instantiate(weapon, currentChild));
    }


    public override void OnStartClient(){
        base.OnStartClient();
        _audioManager = GetComponent<AudioManager>();
    }
}
