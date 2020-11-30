using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Mirror;
public class MatchManagerIPS : NetworkBehaviour
{
    public static MatchManagerIPS Instance;

    public Collider mapBounds;
    public TextMeshProUGUI pickupPrompt;

    public Transform sniperRotationPoint;
    public float ySpawn;

    [Range(5, 75)]
    public float sniperViewDistance = 50f;
    //public float spawnDelay;
    public int maxItems = 3;
    //public GameObject objectToSpawn; 
    public int numOfNPCs = 30;

   // public List<Item_Apple> currentItems = new List<Item_Apple>(); //each new item spawned/picked up is kept track of
    public List<Transform> itemPossibleSpawns = new List<Transform>();


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
       // InvokeRepeating("SpawnItem", spawnDelay, spawnDelay);
        //SpawnNPCs();
        
    }
    // public override void OnStartServer()
    // {
    //     base.OnStartServer();
    //     SpawnNPCs();
    // }


 
 



  

    void SpawnNPCs()
    {


        //Temp spawning: (still needs to be tested)
     
        //int npcSpawnedCounter = 0;
        //attempt to spawn npcs at random point on NavMesh, which I'll need to code eventually...
        //NavMeshHit hit;
        //while (npcSpawnedCounter < numOfNPCs)
        //{
        // if (NavMesh.SamplePosition(transform.position, out hit, 50f, NavMesh.AllAreas))
        // {

        //   npcSpawnedCounter++;
        //  }
        // }

    }


    public Vector3 GetRandomNavMeshPoint(){
        Bounds bounds = mapBounds.bounds;
        Vector3 finalPosition = Vector3.zero;

        Vector3 randomPoint = new Vector3(
            Random.Range(bounds.min.x, bounds.max.x),
            -.16f,
            Random.Range(bounds.min.z, bounds.max.z)
        );
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 5f, 1)) {
             finalPosition = hit.position;            
        }else{
            //keep checking for position
            while(!NavMesh.SamplePosition(randomPoint, out hit, 5f, 1)){
                randomPoint = new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    1f,
                    Random.Range(bounds.min.z, bounds.max.z)
                );
            }
            finalPosition = randomPoint;
        }

        return finalPosition;

    }
}
