using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using Mirror;
using UnityEngine.SceneManagement;

namespace IPS.Inputs
{
    public class RoundManagerIPS : NetworkBehaviour
    {
        public static RoundManagerIPS Instance;

        public GameObject GameEndScreen;
        public int roundTime = 20;

        public int roundNumber;
        [SerializeField] private Animator animator = null;
        [SerializeField] TextMeshProUGUI timerDisplay;

        int currentRoundTime = 20;
        private NetworkManagerIPS room;
        private NetworkManagerIPS Room
        {
            get
            {
                if (room != null) { return room; }
                return room = NetworkManager.singleton as NetworkManagerIPS;
            }
        }

        private void Awake()
        {
            Instance = this;
        }

        public override void OnStartClient()
        {
            currentRoundTime = roundTime;

            DontDestroyOnLoad(gameObject);
        }

        public void CountdownEnded()
        {
            Debug.Log("Countdown Ended Called!!");
            animator.enabled = false;
        }


        #region Server


        public override void OnStartServer()
        {
            currentRoundTime = roundTime;
            NetworkManagerIPS.OnServerStopped += CleanUpServer;
            NetworkManagerIPS.OnServerReadied += CheckToStartRound;
        }

        [ServerCallback]
        private void OnDestroy() => CleanUpServer();

        [Server]
        private void CleanUpServer()
        {
            NetworkManagerIPS.OnServerStopped -= CleanUpServer;
            NetworkManagerIPS.OnServerReadied -= CheckToStartRound;
        }

        [ServerCallback]
        public void StartRound()
        { //called by animator
            currentRoundTime = roundTime;
            RpcStartRound();
            InvokeRepeating("RpcStartCountdown", 1, 1);

        }

        [Server]
        private void CheckToStartRound(NetworkConnection conn)
        {
            //for each game player, checl if they are ready, if the total num of ready players = number of game players, continue on
            if (SceneManager.GetActiveScene().name != "Lobby")
            {
                if (Room.gamePlayers.Count(x => x.connectionToClient.isReady) != Room.gamePlayers.Count) { return; }
                RpcStartAnimation();
            }

            //RpcStartCountdown();
        }



        [ServerCallback]
        void EndRound()
        {
            //Show score scene, Switch Up Player roles, go to new scene,etc
            roundNumber++;
            ScoreManagerIPS.Instance.EndRound();
            RpcEndRound();
        }

        public void EndGame()
        {
            //called by MapHandler
            GameEndScreen.SetActive(true);
        }

        #endregion

        #region Client

        [ClientRpc]
        private void RpcStartAnimation()
        {
            animator.Play("Ani_RoundCountdown", -1, 0f);
            animator.enabled = true;
        }


        [ClientRpc]
        private void RpcStartCountdown()
        {
            if (currentRoundTime > 0)
            {
                currentRoundTime--;
                timerDisplay.text = currentRoundTime.ToString();
            }
            else { EndRound(); }
        }

        [ClientRpc]
        private void RpcStartRound()
        {
            Debug.Log("Start Round -x-x-x-x-x-x-x-x-x-x-x-x-x-x-x");
            InputManager.Remove("Player"); //remove the control block from inputmanager, allowing players to move
        }


        [ClientRpc]
        private void RpcEndRound()
        {
            Debug.Log("End Round");
            CancelInvoke("RpcStartCountdown");
            currentRoundTime = roundTime;


            InputManager.Add("Player");
        }

        #endregion
    }
}