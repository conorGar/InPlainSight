using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


namespace IPS.Inputs
{
    public class Player_ScoreKeeper : NetworkBehaviour
    {

        //Keeps track of the player score locally
        [SyncVar]
        private int currentScore;

        public delegate void ScoreChangedDelegate(int score, GameObject player);
        [SyncEvent]
        public event ScoreChangedDelegate EventScoreChanged;


        #region Server

        [Server]
        private void SetScore(int value)
        {
            currentScore += value;
            EventScoreChanged?.Invoke(currentScore, this.gameObject);
        }

        public override void OnStartServer() => SetScore(0);

        [Command]
        private void CmdAddPoints(int pointsToAdd) => SetScore(pointsToAdd);

        #endregion

        #region Client

        private void Start()
        {
            Debug.Log("PLAYER SCORE KEEPER ADDED TO SCORE MANAGER");
            ScoreManagerIPS.Instance.AddPlayer(this);
        }


        [ClientCallback]
        public void ShootPlayer()
        {//Called by SniperController
            CmdAddPoints(5);
        }

        [ClientCallback]
        public void PlayerCollectItem()
        {
            CmdAddPoints(2);
        }



        //Use DapperDino's health display base to then have ScoreManager have a reference to a UI that shows the scores of all the players and update it

        #endregion
    }

}