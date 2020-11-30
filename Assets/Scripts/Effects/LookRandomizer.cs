using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookRandomizer : MonoBehaviour
{
    public Transform myHead;
    public Transform myFace;
    public Transform myChest;
    public Transform myFeet;
    public GameObject leftSleeve;
    public GameObject rightSleeve;
    public GameObject myMouth;
    [SerializeField] SpriteRenderer leftEye;
    [SerializeField] SpriteRenderer rightEye;

    public GameObject[] possibleHairStyles = new GameObject[1];
    public GameObject[] possibleShirts = new GameObject[1];
    public Color[] possibleHairColors = new Color[0];
    public Color[] possibleEyeColors = new Color[0];
    public Sprite[] possibleMouths = new Sprite[0];
    // Start is called before the first frame update
    private void Awake()
    {
  
        //Spawn Hair
        SpawnBodyPart(possibleHairStyles,possibleHairColors,myHead);

        //Spawn Shirt
        SpawnBodyPart(possibleShirts,possibleHairColors,myChest);

        SetFace();

    }



    void SpawnBodyPart(GameObject[] part, Color[] possibleColors, Transform parent){
        GameObject newPart = Instantiate(part[Random.Range(0, part.Length)]) as GameObject;
        if(parent == myChest){
            //set sleeves for shirt
            rightSleeve.GetComponent<MeshRenderer>().material = newPart.GetComponent<Player_Shirt>().sleeveColor;
            leftSleeve.GetComponent<MeshRenderer>().material = newPart.GetComponent<Player_Shirt>().sleeveColor;

        }
        Vector3 newPartPos = newPart.transform.position;
        Quaternion partRot = newPart.transform.rotation;
        Vector3 partScale = newPart.transform.localScale;
        newPart.transform.parent = parent;//this.gameObject.GetComponent<LookRandomizer>().myHead; // parent; //.SetParent(parent);
        newPart.transform.localPosition = newPartPos;
        newPart.transform.localRotation = partRot;
        newPart.transform.localScale = partScale;
        //newPart.transform.localPosition = Vector3.zero;
        //newPart.GetComponent<MeshRenderer>().material.SetColor("_Color", possibleColors[Random.Range(0, possibleColors.Length)]);
    }

    void SetFace()
    {
        //change eye color and mouth type
        myMouth.GetComponent<SpriteRenderer>().sprite = possibleMouths[Random.Range(0,possibleMouths.Length)];
        int randomEyeColor = Random.Range(0, possibleEyeColors.Length);
        leftEye.color = possibleEyeColors[randomEyeColor];
        rightEye.color = possibleEyeColors[randomEyeColor];

    }
    void SetFeet()
    {

    }

}
