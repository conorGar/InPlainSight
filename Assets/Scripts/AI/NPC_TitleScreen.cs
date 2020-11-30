using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_TitleScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    IEnumerator findNewSpot(){
        yield return new WaitForSeconds(2);
        Vector3 targetPos = Random.insideUnitSphere *10;
        // targetPos = new Vector3()
    }
}
