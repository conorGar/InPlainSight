using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageEventManager_SkiTown : MonoBehaviour
{

    [SerializeField] float eventRepeatRate = 13f;

    [SerializeField] GameObject stormPS;
    [SerializeField] GameObject bus;
    //snow blows heavily every once in a while

    //bus comes and moves across map.

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("StormTimer",eventRepeatRate, eventRepeatRate);
    }

    IEnumerator EventTimer(){
        yield return new WaitForSeconds(Random.Range(1,3));
        int randomEvent = Random.Range(1,4);
        if(randomEvent == 1)
            StartStorm();
        else if(randomEvent == 2)
            StartBus();
    }

    // Update is called once per frame
    void StartStorm(){
        stormPS.SetActive(true);
    }

    void StartBus(){
        bus.SetActive(true);
    }
}
