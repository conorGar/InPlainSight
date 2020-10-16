using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;


namespace IPS.Inputs
{
    public class UI_NetworkLobby : NetworkBehaviour
    {

        [Header("UI")]
        [SerializeField] private GameObject lobbyUI = null;
        [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];
        [SerializeField] private TMP_Text[] playerReadyTexts = new TMP_Text[4];
        public Button startGameButton = null; //set active by networkRoomPlayerIPS

        public UI_StageSelect stageSelectScreen;



        public void ResetPlayerTextDisplays() //called by NetworkRoomPlayerIPS
        {
            //Return all the text to default
            for (int i = 0; i < playerNameTexts.Length; i++)
            {
                playerNameTexts[i].text = "Waiting For Player...";
                playerReadyTexts[i].text = string.Empty;
            }
        }

        public void UpdatePlayerTextDisplays()
        { //called by NetworkRoomPlayerIPS
            for (int i = 0; i < NetworkManagerIPS.Instance.roomPlayers.Count; i++)
            {
                playerNameTexts[i].text = NetworkManagerIPS.Instance.roomPlayers[i].DisplayName;
                playerReadyTexts[i].text = NetworkManagerIPS.Instance.roomPlayers[i].isReady ?
                    "<color=green>Ready</color>" :
                    "<color=red>Not Ready</color>";
            }
        }
    }
}