using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Mirror;

public class Item_Apple : NetworkBehaviour
{
    //when a player enters a trigger for the apple, set 'canpickup' to true in PlayerPickup, set to false when leave trigger
    //BUT ONLY FOR THAT PLAYER

    Transform mySpawnTransform; //used by the scene manager to erase it from occupied spawn positions whem picked up
    
    

    public void SetSpawnTransform(Transform transform)
    {
        mySpawnTransform = transform;

    }

    public Transform GetSpawnTransform()
    {
        return mySpawnTransform;
    }
}
