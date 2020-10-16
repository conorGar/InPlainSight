using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;



namespace IPS.Inputs
{
    public class NetworkManagerIPS : NetworkManager
    {
        public static NetworkManagerIPS Instance;

        [Header("Maps")]
        [SerializeField] public int numberOfRounds = 1;
        private MapSet mapSet = null;
        [SerializeField] private string lobbyScene = "Lobby";
        [SerializeField] private int minPlayers = 2;

        [SerializeField] private GameObject roomPlayerPrefab = null;
        // [SerializeField] private GameObject sniperPlayerPrefab;
        [SerializeField] private GameObject gamePlayerPrefab;

        // [SerializeField] private GameObject gamePlayerPrefab = null;

        [SerializeField] private NetworkPlayerSpawnManager spawnPlayerManager;

        [SerializeField] private ScoreManagerIPS scoreManager;
        [SerializeField] private RoundManagerIPS roundManager = null;


        public List<NetworkRoomPlayerIPS> roomPlayers = new List<NetworkRoomPlayerIPS>();
        public List<NetworkGamePlayer> gamePlayers = new List<NetworkGamePlayer>();

        private MapHandler mapHandler;

        [SerializeField]private bool playingAgain = true; // needed to fix glitch where room player would spawn instantly when game player was destroyed after returning from 'back to lobby' game end screen

        public static event Action OnClientConnected;
        public static event Action OnClientDisconnected;

        //Activated when someone has properly switched scenes on the server
        public static event Action<NetworkConnection> OnServerReadied;
        public static event Action OnServerStopped;
        public static event Action OnChangedScene;


        public override void Awake()
        {
            //if the network manager exists already(if returning to lobby screen from somewhere), destroy this
            // GameObject existingNetworkManager = GameObject.Find("NetworkManager");
            Debug.Log("Does Network manager instance exist?" + NetworkManagerIPS.Instance);
            //gameObject.name = gameObject.name + UnityEngine.Random.Range(0,11);
            if (NetworkManagerIPS.Instance)
            {
                Debug.Log("Destroyed NetworkManager:" + gameObject.name);
                Destroy(this.gameObject);
            }
            else
            {
                Debug.Log("Created Instance of NetworkManager:" + gameObject.name);

                Instance = this;
            }
            base.Awake();
        }

        public override void OnStartClient()
        {
            var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

            foreach (var prefab in spawnablePrefabs)
            {
                ClientScene.RegisterPrefab(prefab);
            }
        }

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);

            OnClientConnected?.Invoke();

        }

        public override void OnClientDisconnect(NetworkConnection conn)
        {
            Debug.Log("------Client Disconnected------");
            base.OnClientDisconnect(conn);

            OnClientDisconnected?.Invoke();

        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            if (SceneManager.GetActiveScene().name == lobbyScene) //&& playingAgain)
            {
                Debug.Log("Server add player got here");

                bool isHost = roomPlayers.Count == 0;

                GameObject roomPlayerInstance = Instantiate(roomPlayerPrefab);

                roomPlayerInstance.GetComponent<NetworkRoomPlayerIPS>().isHost = isHost;

                NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance);

            }

        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            Debug.Log("------Server Disconnected------");

            if (conn.identity != null)
            { //if that client exits..
            Debug.Log("------Server Disconnected  2------");

                NetworkRoomPlayerIPS player = conn.identity.GetComponent<NetworkRoomPlayerIPS>();

                roomPlayers.Remove(player);

                NotifyPlayersOfReadyState();

            }
            base.OnServerDisconnect(conn);
        }

        public override void OnStopServer()
        {
            OnServerStopped?.Invoke();

            roomPlayers.Clear();
            gamePlayers.Clear();
        }

        public void NotifyPlayersOfReadyState()
        {
            foreach (NetworkRoomPlayerIPS player in roomPlayers)
            {
                player.HandleReadyToStart(IsReadyToStart());
            }
        }

        private bool IsReadyToStart()
        {

            if (numPlayers < minPlayers) { return false; }

            foreach (NetworkRoomPlayerIPS player in roomPlayers)
            {
                if (!player.isReady) { return false; }
            }

            return true;
        }

        public void SetMapSetAndLeaveLobby(MapSet maps)
        { //called by UI_StageSelect
            mapSet = maps;
            LeaveLobbyStartGame();
        }

        public void LeaveLobbyStartGame()
        {
            if (SceneManager.GetActiveScene().name == lobbyScene)
            {
                if (!IsReadyToStart()) { return; }

                mapHandler = new MapHandler(mapSet, numberOfRounds);

                ServerChangeScene(mapHandler.NextMap);
            }
        }

        public void ReturnToLobby(bool playingAgain)
        {
            RoundManagerIPS.Instance.GameEndScreen.SetActive(false);
            this.playingAgain = playingAgain;
            Debug.Log("DOES SPAWN MANAGER EXIST?" + NetworkPlayerSpawnManager.Instance);

            ServerChangeScene("Lobby");
            //I guess remove the spawn manager, score system and round system

            Debug.Log("Returning to Lobby got here, round manager and score manager should be destroyed" + playingAgain);
            Destroy(RoundManagerIPS.Instance.gameObject);
            Destroy(ScoreManagerIPS.Instance.gameObject);
            foreach (NetworkGamePlayer player in gamePlayers)
            {
                Destroy(player.gameObject);
            }

            if (playingAgain)
            {
                

            }
            else
            {
                Debug.Log(NetworkPlayerSpawnManager.Instance + "< Does the spawn player manager currently exist?");
                //destory the host/clients/spawners or whatever I need to do for that.
                //StopClient();
                //NetworkServer.DisconnectAllConnections();
                NetworkPlayerSpawnManager.Instance.OnNetworkDestroy();
                NetworkManagerIPS.Instance.StopHost();
                playingAgain = true;
                                Debug.Log("NOT PLAYING AGAIN, RETURNING TO LOBBY" + playingAgain);


            }
                                    Destroy(NetworkPlayerSpawnManager.Instance.gameObject);


        }

        public void GoToScene(string sceneName)
        {
            if (!IsReadyToStart()) { return; }

            ServerChangeScene(sceneName);
        }

        public override void ServerChangeScene(string newSceneName)
        {
            //from lobby to game
            if (SceneManager.GetActiveScene().name == lobbyScene)
            {


                // ---------- Set up sniper players -----------------//
                List<NetworkRoomPlayerIPS> sniperPlayers = new List<NetworkRoomPlayerIPS>();
                int numberOfSnipers = 1;

                //there's a sniper player for <3,5,7+ players

                numberOfSnipers += Mathf.FloorToInt((roomPlayers.Count - 3) / 2);
                if (numberOfSnipers < 1)
                    numberOfSnipers = 1;


                for (int i = 0; i < numberOfSnipers; i++)
                {
                    //which player is the sniper is random(for now)
                    int randomPlayer = UnityEngine.Random.Range(0, roomPlayers.Count);
                    while (sniperPlayers.Contains(roomPlayers[randomPlayer]))
                    {
                        //if this player is already a sniper, find a new one
                        randomPlayer = UnityEngine.Random.Range(0, roomPlayers.Count);
                    }
                    Debug.Log("Sniper role assigned to room player #:" + randomPlayer);
                    sniperPlayers.Add(roomPlayers[randomPlayer]);

                }





                //the leftover players are all hiders
                for (int i = roomPlayers.Count - 1; i >= 0; i--)
                {


                    NetworkConnection conn = roomPlayers[i].connectionToClient;
                    GameObject gameplayerInstance = Instantiate(gamePlayerPrefab);

                    if (sniperPlayers.Contains(roomPlayers[i]))
                    {
                        gameplayerInstance.GetComponent<NetworkGamePlayer>().myPlayerType = NetworkGamePlayer.PLAYER_TYPE.SNIPER;
                    }
                    else
                    {
                        gameplayerInstance.GetComponent<NetworkGamePlayer>().myPlayerType = NetworkGamePlayer.PLAYER_TYPE.HIDER;

                    }
                    NetworkServer.Destroy(conn.identity.gameObject);
                    Debug.Log("-q-q-q-q-q-q- REPLACE PLAYER CONNECTION LOOP RAN q-q-q-q-q-q-q-q---q");
                    //the player for the current client is NO LONGER what we just destroyed but this new gameobject
                    NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
                }



                GameObject scoreSystemInstance = Instantiate(scoreManager.gameObject);
                NetworkServer.Spawn(scoreSystemInstance);

                GameObject roundSystemInstance = Instantiate(roundManager.gameObject);
                NetworkServer.Spawn(roundSystemInstance);

            }

            base.ServerChangeScene(newSceneName);
        }

        public void ServerChangeScene()
        { //called by ScoreManager when going to 
            if (!mapHandler.IsComplete)
                base.ServerChangeScene(mapHandler.NextMap);
            else
                RoundManagerIPS.Instance.EndGame();

        }

        public override void OnServerSceneChanged(string sceneName)
        {
            //create the player spawner once the scene has changed for this client
            Debug.Log("Server changed scene activated!_!_!_!__!_!_!_!__!_!" + sceneName + playingAgain);
            // if (sceneName == "Assets/Scenes/SampleScene.unity")
            // {

            if (sceneName != "Lobby")
            {
                Debug.Log("OnserverSceneChanged(). current scene:" + sceneName);
                GameObject spawnSystemInstance = Instantiate(spawnPlayerManager.gameObject);
                NetworkServer.Spawn(spawnSystemInstance);

            }
            else if (playingAgain)
            {
                NetworkLobby.Instance.SkipLandingPage();
          
            }
            // else if (!playingAgain)
            // {
            //     playingAgain = true;

            // }

        }


        public override void OnServerReady(NetworkConnection conn)
        {
            Debug.Log("!-!-!-!-!-!-! ON SERVER READY CALLED !-!-!-!-!-!-!");

            base.OnServerReady(conn);

            //? is to make sure it's not null
            OnServerReadied?.Invoke(conn);
        }

 
    }

}