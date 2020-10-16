using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Mirror;
using System.Linq;


namespace IPS.Inputs
{
    public class ScoreManagerIPS : NetworkBehaviour
    {
        public static ScoreManagerIPS Instance;

        public Dictionary<GameObject,int> currentPlayers = new Dictionary<GameObject, int>();
        //List<int> scores = new List<int>();
        public List<UI_PlayerScoreDisplay> scoreDisplays = new List<UI_PlayerScoreDisplay>();
        public GameObject playerScoreDisplayPrefab;

        public GameObject roundEndDisplay;

        private NetworkManagerIPS room;
        private NetworkManagerIPS Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerIPS;
            }
        }

        [SyncEvent]
        public event Action EventScoreChanged;


        void Awake()
        {
            Instance = this;
        }

        public List<GameObject> GetWinners(){
            List<GameObject> winners = new List<GameObject>();
            int currentHighest = currentPlayers.Values.ElementAt(0);



            //Get Highest score
            foreach(int playerScore in currentPlayers.Values){
                if(playerScore > currentHighest){
                    currentHighest = playerScore;
                }
            }

            for(int i = 0; i < currentPlayers.Count; i++){
                if(currentPlayers.Values.ElementAt(i) == currentHighest){
                    winners.Add(currentPlayers.Keys.ElementAt(i));
                }
            }

            return winners;

        }

        #region Server


        public override void OnStartServer()
        {
            NetworkManagerIPS.OnServerStopped += CleanUpServer;

        }
        public override void OnStartClient()
        {
            DontDestroyOnLoad(gameObject);

        }

        public void AddPlayer(Player_ScoreKeeper player)
        { //called by Player_ScoreKeeper on creation
            player.EventScoreChanged += HandleScoreChange;
            currentPlayers.Add(player.gameObject,0);
            Debug.Log("Player added to score manager" + currentPlayers);

        }

        [ServerCallback]
        private void OnDestroy()
        {
            CleanUpServer();
        }

        [Server]
        private void CleanUpServer()
        {

            foreach (GameObject player in currentPlayers.Keys)
            {
                player.GetComponent<Player_ScoreKeeper>().EventScoreChanged -= HandleScoreChange;
            }

            NetworkManagerIPS.OnServerStopped -= CleanUpServer;


        }





        private void HandleScoreChange(int newScore, GameObject player)
        {
            //called by Player_ScoreKeeper whenever that player's score is changed
            Debug.Log("HandleScoreChange activated!");


            //at the SAME index in another list: PlayerScoreDisplays(filled at same time ) set the display number to this index's Player_ScoreKeeper.currentScore
            if (!currentPlayers.Keys.Contains(player))
            {
                Debug.Log("Player for handle score change does not exist in score manager!!");
                return;
            }

            //update score
            currentPlayers[player] = newScore;



        }


        [Server]
        public void EndRound()
        { //called by the round manager
            roundEndDisplay.SetActive(true);

            //for each player, spawn a player score display and set the needed values based on that player
            foreach (GameObject player in currentPlayers.Keys)
            {
                NetworkConnection conn = player.GetComponent<Player_ScoreKeeper>().connectionToClient;
                Debug.Log(conn.identity);

                GameObject scoreDisplayInstance = Instantiate(playerScoreDisplayPrefab, roundEndDisplay.transform, false);
                //Debug.Log("index:" + index + "current score:" + scores[index]);

                NetworkServer.Spawn(scoreDisplayInstance, conn);
                scoreDisplayInstance.GetComponent<UI_PlayerScoreDisplay>().RpcSetScoreDisplay(currentPlayers[player]);




            }
            RpcShowScores();
        }
        [ClientRpc]
        void RpcShowScores()
        {
            roundEndDisplay.SetActive(true);

        }

        public void NotifyPlayersOfReadyState()
        {
            foreach (UI_PlayerScoreDisplay player in scoreDisplays)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }

        private bool IsReadyToStart()
        {


            foreach (UI_PlayerScoreDisplay player in scoreDisplays)
            {
                if (!player.isReady) { return false; }
            }

            return true;
        }


        public void LeaveLobbyStartGame()
        {
            Debug.Log("Score manager leavelobbystartgame() called");
            if (!IsReadyToStart()) { return; }

            //remove score display
            roundEndDisplay.SetActive(false);
            foreach (UI_PlayerScoreDisplay player in scoreDisplays)
            {

                Destroy(player);
            }
            currentPlayers.Clear();
            RpcClearScoreDisplay();

            NetworkManagerIPS.Instance.ServerChangeScene();



        }

        [ClientRpc]
        void RpcClearScoreDisplay()
        {
            roundEndDisplay.SetActive(false);
        }



        #endregion


    }

}