using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;



namespace IPS.Inputs
{
    public class NetworkRoomPlayerIPS : NetworkBehaviour
    {
        public UI_NetworkLobby networkLobbyUI;
        public GameObject lobbyUIHolder;
        //SyncVar = variables that can only be changed on the server
        //hook = name of a method that is called when this variable changes
        [SyncVar(hook = nameof(HandleDisplayNameChanged))]
        public string DisplayName = "Loading...";

        [SyncVar(hook = nameof(HandleReadyStatusChanged))]
        public bool isReady = false;

        public bool isHost;

        public override void OnStartAuthority()
        {

            CmdSetDisplayName(UI_PlayerNameInput.DisplayName);
            lobbyUIHolder.SetActive(true);
        }

        public override void OnStartClient()
        {

            NetworkManagerIPS.Instance.roomPlayers.Add(this);

            UpdateDisplay();
        }

        public override void OnNetworkDestroy()
        {
            NetworkManagerIPS.Instance.roomPlayers.Remove(this);
            UpdateDisplay();
        }

        public void LeaveLobby()
        {
            NetworkManagerIPS.Instance.roomPlayers.Remove(this);
            //UpdateDisplay();
            if (isHost)
            {
            
                RpcLeaveLobby();
                NetworkManagerIPS.Instance.StopHost();

            }
            else
            {
                RpcLeaveLobby();


            }
            NetworkLobby.Instance.landingPageHolder.SetActive(true);
            this.gameObject.SetActive(false);

        }

        public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
        public void HandleDisplayNameChanged(string oldValue, string newValue) => UpdateDisplay();

        public virtual void UpdateDisplay()
        {
            if (!hasAuthority) //if not the local player being updated...
            {
                foreach (var player in NetworkManagerIPS.Instance.roomPlayers)
                {
                    if (player.hasAuthority)
                    {
                        player.UpdateDisplay();
                        break;
                    }
                }

                return;
            }

            //return all the text to default for a sec..
            networkLobbyUI.ResetPlayerTextDisplays();

            networkLobbyUI.UpdatePlayerTextDisplays();

        }


        public virtual void HandleReadyToStart(bool readyToStart)
        {
            if (!isHost) { return; }

            networkLobbyUI.startGameButton.gameObject.SetActive(readyToStart);
            networkLobbyUI.startGameButton.interactable = readyToStart;
        }

        [Command]
        private void CmdSetDisplayName(string displayName)
        {
            DisplayName = displayName;
        }

        [Command]
        public virtual void CmdReadyUp()
        {
            isReady = !isReady;

            NetworkManagerIPS.Instance.NotifyPlayersOfReadyState();
        }

        [Command]
        public virtual void CmdStartGame()
        {
            // Debug.Log("Got here start game");
            // //if this person is NOT the firs t person in the room(host), then return
            // if (NetworkManagerIPS.Instance.roomPlayers[0].connectionToClient != connectionToClient) { return; }

            // Debug.Log("Got here start game 2");

            // NetworkManagerIPS.Instance.LeaveLobbyStartGame();
        }

        [ClientRpc]
        void RpcLeaveLobby()
        {
            Debug.Log("Leave Lobby for client activated");
            NetworkManagerIPS.Instance.StopClient();
            NetworkManagerIPS.Instance.roomPlayers.Clear();
            NetworkLobby.Instance.landingPageHolder.SetActive(true);

        }


        [Command]
        public void CmdStartStageSelect()
        {
            networkLobbyUI.stageSelectScreen.gameObject.SetActive(true);
        }




    }

}