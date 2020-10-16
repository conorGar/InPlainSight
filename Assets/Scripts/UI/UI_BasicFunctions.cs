using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_BasicFunctions : MonoBehaviour
{

    //A series of functions usually called by the gameobject's animator

    // Start is called before the first frame update
    public void DisableSelf(){
        this.gameObject.SetActive(false);
    }
}
