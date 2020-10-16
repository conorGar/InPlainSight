using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;



namespace IPS.Inputs
{
    public class NetworkGamePlayer : NetworkBehaviour
    {
        [SyncVar]
        public string displayName = "Loading...";

        public enum PLAYER_TYPE
        {
            HIDER,
            SNIPER
        };

        public PLAYER_TYPE myPlayerType;
        private NetworkManagerIPS room;
        private NetworkManagerIPS Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerIPS;
            }
        }

        public override void OnStartClient()
        {
            DontDestroyOnLoad(gameObject);

            Room.gamePlayers.Add(this);
        }

        public override void OnNetworkDestroy()
        {
            Debug.Log("On Network Destroy activated for game player");
            Room.gamePlayers.Remove(this);
        }

    
        [Server]
        public void SetDisplayName(string displayName)
        {
            this.displayName = displayName;
        }

    }
}