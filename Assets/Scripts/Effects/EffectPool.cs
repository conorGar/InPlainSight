using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPool : MonoBehaviour
{

    public static EffectPool Instance;

    public List<GameObject> effects = new List<GameObject>();


    List<GameObject> effectInstances = new List<GameObject>();

    
    
    void Awake(){
        Instance = this;

        for (int j=0; j < effects.Count; ++j)
            {
                var obj = Instantiate(effects[j]) as GameObject;
                obj.SetActive(false);
                effectInstances.Add(obj);
                //pooledObjects[poolDefinition].Add(obj);

               
            }

    }

        public GameObject GetPooledObject(string name, Vector3 pos = new Vector3(), bool setActive = true)
    {
        GameObject obj = GetInternalPooledObject(name);
        if (obj != null) {
            Debug.Log("Found pooled object" + obj.name);
            obj.transform.SetPositionAndRotation(pos, Quaternion.identity);
            obj.SetActive(setActive);
        }

        return obj;
    }

    private GameObject GetInternalPooledObject(string tag)
    {
        Debug.Log("Got here- pooled object");
        GameObject poolDefinition = null;
        for (int i = 0; i < effects.Count; ++i) {
            if (effectInstances[i].tag == tag) {
                poolDefinition = effectInstances[i];
                break;
            }
        }

        if (poolDefinition != null) {
                    Debug.Log("Got here- pooled object-2-");

            // var objects = pooledObjects[poolDefinition];
            // if (objects != null) {
            //     for (int i = 0; i < objects.Count; ++i) {
            //         if (!objects[i].activeInHierarchy) {
            //             return objects[i];
            //         }
            //     }

            //     if (poolDefinition.IsExpandable) {
            //         GameObject obj = Instantiate(poolDefinition.poolObject) as GameObject;
            //         pooledObjects[poolDefinition].Add(obj);

            //         if (poolDefinition.parentObject != null) {
            //             obj.transform.parent = poolDefinition.parentObject.transform;
            //         }

            //         return obj;
            //     }
            // }
            return poolDefinition;
        }

        Debug.LogError("Requested a pooled object [" + name + "] but could not retrieve it.");
        return null;
    }
   
}
