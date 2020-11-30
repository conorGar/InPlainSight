using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

namespace IPS.Inputs
{
    public class UI_PlayerScoreDisplay : NetworkRoomPlayerIPS
    {

        //TODO: Assign the width of the progress bar according to how many players are in the game currently
        //>proper player icon display
        //> Play rising animation based on each player's score relative to every other player's score

        public TextMeshProUGUI readyText;

        public int myScore;

        [SerializeField] GameObject progressBar;
        [SerializeField] TextMeshProUGUI nameDisplay;
        [SerializeField] TextMeshProUGUI scoreNumberDisplay;

        float percentageOfFill; // set by score manager at round end 
        string playerName;
        bool isRising;
        Vector2 targetRisePosition;
        public override void OnStartAuthority()
        {
          

            return;
        }

        private void Start() {
            
            this.transform.parent = ScoreManagerIPS.Instance.roundEndDisplay.transform;
            this.transform.localPosition = new Vector2(this.transform.localPosition.x + (ScoreManagerIPS.Instance.scoreDisplays.Count * 20), this.transform.localPosition.y);
        }
        void Update()
        {

            if(isRising){
                Debug.Log("isRising..." + progressBar.transform.localPosition.y);
                progressBar.GetComponent<RectTransform>().anchoredPosition = Vector2.MoveTowards(progressBar.GetComponent<RectTransform>().anchoredPosition, targetRisePosition, 115*Time.deltaTime);
                if(Mathf.Abs(targetRisePosition.y - progressBar.transform.position.y) < 2f){
                    Debug.Log("Bar stopped!");
                    isRising = false;
                } 
            }

            if (Input.GetKeyDown(KeyCode.Space) && !isRising)
            {
                Debug.Log("HERE!!!!!!!!");
                Debug.Log("isLocalPlayer? " + isLocalPlayer);
                Debug.Log("isServer? " + isServer);
                Debug.Log("isClient? " + isClient);
                Debug.Log("hasAuthority? " + hasAuthority);
                Debug.Log("isReady? " + isReady);

                if (hasAuthority)
                {
                    Debug.Log("Got here- Player score display continue");
                    //press actionkey to 'ready up' and start the next round
                    CmdReadyUp();
                    if (ScoreManagerIPS.Instance.scoreDisplays[0] == this)
                    { //if I am the host
                        ScoreManagerIPS.Instance.LeaveLobbyStartGame();
                    }
                }
            }
        }

        public override void UpdateDisplay()
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

            //TODO: actually change display
        }

        [Command]
        public override void CmdStartGame()
        {
            //if this person is NOT the firs t person in the room(host), then return
            if (NetworkManagerIPS.Instance.roomPlayers[0].connectionToClient != connectionToClient) { return; }


            NetworkManagerIPS.Instance.GoToScene("Stage_Temple");
        }

        [ClientRpc]
        public void RpcSetScoreDisplay(int score)
        { 
            myScore = score;
            scoreNumberDisplay.text = score.ToString();
        }

          [ClientRpc]
        public void RpcSetFillPercentage(float percentage)
        {
            
            percentageOfFill = percentage;
            Debug.Log("Fill percentage: " + percentage);

            targetRisePosition = new Vector2(progressBar.GetComponent<RectTransform>().anchoredPosition.x,  -113 - (263*(1-percentageOfFill)));
            Debug.Log("TargetRisePosition:" + targetRisePosition);
        }
         [ClientRpc]
        public void RpcStartRising()
        { 
            isRising = true;
        }
    }
}