using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;

namespace IPS.Inputs
{
    public class UI_GameEndScreen : NetworkBehaviour
    {
        [SerializeField] TextMeshProUGUI winnerNameDisplay;
        [SerializeField] GameObject waitingForHostDisplay;

        [SerializeField] GameObject playerGameEndPrefab;

        public List<GameObject> currentPlayers = new List<GameObject>(); //to keep track of game player connections

        //when play again button is pressed, return to the lobby but skip the initial join/host part somehow(with all current players)
        //back to lobby just returns to the lobby

        //the only one that can do this is the host, for all other players, a "waiting for host" popup appears if press on of these buttons

        void OnEnable(){
            //use the score manager to get the current players and their connections
            Debug.Log("****  GOT HERE - GAME END SCREEN!!  ***");
              foreach (NetworkGamePlayer player in NetworkManagerIPS.Instance.gamePlayers)
            {
                            Debug.Log("****  GOT HERE - GAME END SCREEN!!  2  ***");

                NetworkConnection conn = player.connectionToClient;
                Debug.Log(conn.identity);

                GameObject playerInstance = Instantiate(playerGameEndPrefab);
                playerInstance.GetComponent<NetworkGameEndPlayer>().gameEndScreen = this;

                NetworkServer.Spawn(playerInstance, conn);

                currentPlayers.Add(playerInstance);


            }
        }

        void DisplayWinner()
        {
            List<GameObject> winners = ScoreManagerIPS.Instance.GetWinners();
            if (winners.Count == 1)
            {
                winnerNameDisplay.text = winners[0].GetComponent<NetworkGamePlayer>().displayName;
            }
            else
            {
                //TODO: Show multiple player win displays
            }
        }


    }
}