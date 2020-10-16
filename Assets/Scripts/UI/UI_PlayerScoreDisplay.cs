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

        [SerializeField] TextMeshProUGUI nameDisplay;
        [SerializeField] TextMeshProUGUI scoreNumberDisplay;

        string playerName;
        public override void OnStartAuthority()
        {
          

            return;
        }

        private void Start() {
              ScoreManagerIPS.Instance.scoreDisplays.Add(this);
            this.transform.parent = ScoreManagerIPS.Instance.roundEndDisplay.transform;
            this.transform.localPosition = new Vector2(this.transform.localPosition.x + (ScoreManagerIPS.Instance.scoreDisplays.Count * 20), this.transform.localPosition.y);
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
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
            scoreNumberDisplay.text = score.ToString();
        }
    }
}