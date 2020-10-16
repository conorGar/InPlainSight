using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Mirror;

namespace IPS.Inputs
{

    public class NetworkPlayerSpawnManager : NetworkBehaviour
    {

        public static NetworkPlayerSpawnManager Instance;
        [SerializeField] private GameObject hiderPlayerPrefab;
        [SerializeField] private GameObject sniperPlayerPrefab;

        [SerializeField] private GameObject npcPrefab;

        [SerializeField] private GameObject itemPrefab;


        public List<Item_Apple> currentItems = new List<Item_Apple>(); //each new item spawned/picked up is kept track of
                                                                       //public List<Transform> itemPossibleSpawns = new List<Transform>();

        List<Transform> currentlyOccupiedItemSpawns = new List<Transform>();

        void Awake()
        {
            Instance = this;
        }
        public override void OnStartServer()
        {
            NetworkManagerIPS.OnServerReadied += SpawnPlayer;

            SpawnItem();
            SpawnNPCS();



        }

        public override void OnStartClient()
        {
            //Prevent player from moving during 3-2-1-GO! sequence
            InputManager.Add("Player"); //disable all input
            InputManager.Controls.Player.Look.Enable(); //specifically enable looking
        }

        [ServerCallback]
        private void OnDestroy()
        {
            NetworkManagerIPS.OnServerReadied -= SpawnPlayer;
            Debug.Log("NETWORK PLAYER SPAWNER HAS BEEN DESTORYED!");
        }

        public override void OnNetworkDestroy()
        {
            NetworkManagerIPS.OnServerReadied -= SpawnPlayer;
            Debug.Log("NETWORK PLAYER SPAWNER HAS BEEN DESTORYED! 2");
            base.OnNetworkDestroy();
        }

        public override void OnStopClient()
        {
            NetworkManagerIPS.OnServerReadied -= SpawnPlayer;
            Debug.Log("NETWORK PLAYER SPAWNER HAS BEEN DESTORYED! 3333333333333");
            base.OnStopClient();
        }



        [Server]
        public void SpawnPlayer(NetworkConnection conn)
        {

            Debug.Log("Spawn Player called! -x-x-x-x-x-x-x-x-x-x-x-x-x-x" + NetworkPlayerSpawnManager.Instance);
            //Pick a random, available point on the navmesh and spawn player
            Vector3 spawnPoint = MatchManagerIPS.Instance.GetRandomNavMeshPoint();
            spawnPoint = new Vector3(spawnPoint.x, .37f, spawnPoint.z); //ensure player is above the ground when spawned
            GameObject playerInstance = null;


            if (conn.identity.GetComponent<NetworkGamePlayer>().myPlayerType == NetworkGamePlayer.PLAYER_TYPE.HIDER)
            {
                Debug.Log("Successfully read player as a hider");
                playerInstance = Instantiate(hiderPlayerPrefab, spawnPoint, Quaternion.identity);
                playerInstance.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

            }
            else
            {
                Debug.Log("Successfully read player as a sniper");
                playerInstance = Instantiate(sniperPlayerPrefab, spawnPoint, Quaternion.identity);
            }

            // Quaternion spawnRot = Quaternion.Euler(-90f, 0f, 0f);
            // playerInstance = Instantiate(hiderPlayerPrefab, spawnPoint, spawnRot);

            NetworkServer.Spawn(playerInstance, conn);
            Debug.Log(playerInstance.transform.rotation + "<- ROTATION AT PLAYER SPAWN");



        }

        [Server]
        public void SpawnItem()
        {
            Debug.Log("Got here- spawn item");

            if (currentlyOccupiedItemSpawns.Count < MatchManagerIPS.Instance.maxItems)
            {
                Debug.Log("Got here- spawn item 2");
                int listIndex = Random.Range(0, MatchManagerIPS.Instance.itemPossibleSpawns.Count);

                if (!currentlyOccupiedItemSpawns.Contains(MatchManagerIPS.Instance.itemPossibleSpawns[listIndex]))
                {
                    //spawn
                    GameObject item = Instantiate(itemPrefab, MatchManagerIPS.Instance.itemPossibleSpawns[listIndex].position, Quaternion.identity);
                    item.GetComponent<Item_Apple>().SetSpawnTransform(MatchManagerIPS.Instance.itemPossibleSpawns[listIndex]);

                    currentItems.Add(item.GetComponent<Item_Apple>());
                    NetworkServer.Spawn(item);

                    currentlyOccupiedItemSpawns.Add(MatchManagerIPS.Instance.itemPossibleSpawns[listIndex]);

                }
                else
                {
                    //TODO: replace with mirror spawn point system

                    // while (currentlyOccupiedItemSpawns.Contains(itemPossibleSpawns[listIndex]))
                    // {
                    //     //keep searching for an unoccupied spawn point
                    //     listIndex = Random.Range(0, itemPossibleSpawns.Count);
                    // }

                }
            }


        }

        [Server]
        public void SpawnNPCS()
        {
            for (int i = 0; i < MatchManagerIPS.Instance.numOfNPCs; i++)
            {


                Vector3 spawnPos = MatchManagerIPS.Instance.GetRandomNavMeshPoint();
                spawnPos = new Vector3(spawnPos.x, .37f, spawnPos.z); //make sure npc is above the ground when spawned
                GameObject npc = Instantiate(npcPrefab, spawnPos, Quaternion.identity);
                npc.transform.rotation = Quaternion.Euler(-90f, 0f, 0f);

                NetworkServer.Spawn(npc);
            }
        }

        public void RemoveItem(Item_Apple item)
        {


            if (currentItems.Contains(item))
            { //if the item is STILL in the list when this is called
                foreach (Item_Apple apple in currentItems)
                {
                    if (apple == item)
                    {
                        currentlyOccupiedItemSpawns.Remove(apple.GetSpawnTransform());
                        currentItems.Remove(apple);
                        Destroy(apple.gameObject);
                        MatchManagerIPS.Instance.pickupPrompt.gameObject.SetActive(false);
                        break;
                    }
                }
            }
        }
    }
}