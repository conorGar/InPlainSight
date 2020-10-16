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
