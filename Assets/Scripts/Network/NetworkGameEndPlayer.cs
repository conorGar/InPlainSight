using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;



namespace IPS.Inputs
{
    public class NetworkGameEndPlayer : NetworkBehaviour
    {

        [SerializeField] GameObject clientDisplay;
        [SerializeField] GameObject hostDisplay;

        public UI_GameEndScreen gameEndScreen; //Set by gameEndScreen on creation
        // Start is called before the first frame update


        private void Start()
        {
                        Debug.Log("****  GOT HERE - GAME END SCREEN!!  3 ***");

            if (gameEndScreen.currentPlayers[0].GetComponent<NetworkGameEndPlayer>().connectionToClient == connectionToClient)
            {
                hostDisplay.SetActive(true);
            }
            else
            {
                clientDisplay.SetActive(true);
            }

        }
        public void ReturnToLobby()
        {


            NetworkManagerIPS.Instance.ReturnToLobby(false);


        }

        public void PlayAgain()
        {
            Debug.Log("Playing again, Return to lobby called with TRUE value");
            NetworkManagerIPS.Instance.ReturnToLobby(true);


        }
    }
}