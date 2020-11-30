using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_TitleScreen : MonoBehaviour
{
    enum STATES
    {
        INTRO,
        MAIN,
        LOBBY
    }
    [SerializeField] float transitionSpeed = 20f;
    [SerializeField] List<Transform> camPostions = new List<Transform>();
    [SerializeField] Camera mainCam;

    [SerializeField] STATES currentState = STATES.INTRO;

    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject pressAnyButtonText;

    [SerializeField] GameObject lobbyMenu;


    Vector3 targetRot;
    void Awake()
    {
        mainMenu.SetActive(false);
        pressAnyButtonText.SetActive(true);
        lobbyMenu.SetActive(false);
    }

    void Start()
    {
     
        

    }

    void Update()
    {
           if (currentState == STATES.MAIN)
        {
            targetRot = new Vector3 (22.7f, -163.33f, -3.37f);

        }
        else if (currentState == STATES.LOBBY)
        {

            targetRot = new Vector3 (13.64f, -64.55f, 0.24f);

        }
        else if (currentState == STATES.INTRO)
        {


            targetRot = new Vector3(22.75f, -148.68f, 2.54f);


            //target.position
            if (Input.anyKeyDown)
            {
                Debug.Log("Any key is pressed!");
                pressAnyButtonText.SetActive(false);
                currentState = STATES.MAIN;
                mainMenu.SetActive(true);
            }
        }
        mainCam.transform.rotation = Quaternion.RotateTowards(mainCam.transform.rotation,Quaternion.Euler(targetRot.x,targetRot.y,targetRot.z),transitionSpeed*Time.deltaTime);
    }

    public void GoToLobby(){
        currentState = STATES.LOBBY;
        mainMenu.SetActive(false);
        lobbyMenu.SetActive(true);
    }

    public void Back(){
         if (currentState == STATES.LOBBY)
        {
            lobbyMenu.SetActive(false);
            currentState = STATES.MAIN;
            mainMenu.SetActive(true);
        }
    }
}
