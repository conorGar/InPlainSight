using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookRandomizer : MonoBehaviour
{
    public Transform myHead;
    public Transform myFace;
    public Transform myChest;
    public Transform myFeet;

    public GameObject[] possibleHairStyles = new GameObject[1];
    public GameObject[] possibleShirts = new GameObject[1];
    public Color[] possibleHairColors = new Color[0];
    // Start is called before the first frame update
    private void Awake()
    {
  
        //Spawn Hair
        SpawnBodyPart(possibleHairStyles,possibleHairColors,myHead);

        //Spawn Shirt
        SpawnBodyPart(possibleShirts,possibleHairColors,myChest);

    }



    void SpawnBodyPart(GameObject[] part, Color[] possibleColors, Transform parent){
        GameObject newPart = Instantiate(part[Random.Range(0, part.Length)]);
        Vector3 newPartPos = newPart.transform.position;
        Quaternion partRot = newPart.transform.rotation;
        Vector3 partScale = newPart.transform.localScale;
        newPart.transform.SetParent(parent);
        newPart.transform.localPosition = newPartPos;
        newPart.transform.localRotation = partRot;
        newPart.transform.localScale = partScale;
        //newPart.transform.localPosition = Vector3.zero;
        newPart.GetComponent<MeshRenderer>().material.SetColor("_Color", possibleColors[Random.Range(0, possibleColors.Length)]);
    }

    void SetFace()
    {
        //change eye color and mouth type
    }
    void SetFeet()
    {

    }

}
