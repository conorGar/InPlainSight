using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;

public class HiderPlayer_Health : NetworkBehaviour
{
    [SyncEvent]
    public event Action EventPlayerShot;
    bool isAlive;

    #region Server
    public override void OnStartServer() => isAlive = true;

    [Command]
    private void CmdPlayerShot(){
        EventPlayerShot?.Invoke();
        //TODO: Remove/Respawn player

        //pick a random one of the NPCS

        //Get their random clothes(will need a clothes reference on LookRandomizer?)

        //Replace this player's clothes with that random NPC's clothes

        //move this player to that NPC's position

        //remove the NPC

        //activate the player
    }
    #endregion

    #region Client

    //maybe have method that is called by the sniper when shot, hopefully this works
    [ClientCallback]
    public void ShotBySniper(){
        CmdPlayerShot();
    }

    #endregion
}
