using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TitleScreenManager : MonoBehaviour
{
        public static TitleScreenManager Instance;
        public Collider walkZone;

        public List<GameObject> NPCs = new List<GameObject>();

        void Awake(){
            Instance = this;
        }

        void Start(){
            foreach(GameObject npc in NPCs){
                npc.SetActive(true);
            }
        }

        public Vector3 GetRandomNavMeshPoint(){
        Bounds bounds = walkZone.bounds;
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
