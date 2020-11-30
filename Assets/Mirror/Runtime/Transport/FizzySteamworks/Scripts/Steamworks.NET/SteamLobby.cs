using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Mirror;


namespace IPS.Inputs
{
    public class SteamLobby : MonoBehaviour
    {
        // Start is called before the first frame update

        protected Callback<LobbyCreated_t> lobbyCreated;
        protected Callback<GameLobbyJoinRequested_t> gameLobbyJoinRequested;
                protected Callback<LobbyEnter_t> lobbyEntered;



        private const string HostAdressKey = "HostAddress";

        [SerializeField] NetworkManager networkManager;
        private void Start()
        {
            if (!SteamManager.Initialized) { return; } //if steam isn't even open, then don't do anything else

            lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
            gameLobbyJoinRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
            lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEntered);


        }
        public void HostLobby()
        {

            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 8);
        }


        private void OnLobbyCreated(LobbyCreated_t callback)
        {

            if (callback.m_eResult != EResult.k_EResultOK)
            {
                return; //did not successfully create the lobby
            }

            networkManager.StartHost();

            SteamMatchmaking.SetLobbyData(new CSteamID(callback.m_ulSteamIDLobby), HostAdressKey, SteamUser.GetSteamID().ToString());




        }
        private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
        {
            SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
        }

        void OnLobbyEntered(LobbyEnter_t callback){
            if(NetworkServer.active){return;} //if the server is already active(ie: is Host) return

            //grab host address
            string HostAddress = SteamMatchmaking.GetLobbyData(new CSteamID(callback.m_ulSteamIDLobby),HostAdressKey);

            networkManager.networkAddress = HostAddress;
            networkManager.StartClient();
        }
    }


}