using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;


namespace IPS.Inputs
{
    public class UI_StageSelect : NetworkBehaviour
    {

        [SerializeField] TextMeshProUGUI stageTitleDisplay;

        //have a list that different stages are added to/removed from as they are clicked.
        public List<UI_StageSelectOption> selectedStages = new List<UI_StageSelectOption>();
        //once everything is set, press a PLAY button to start.


        public void CreateMap()
        { //activated on start button press
            if (selectedStages.Count > 0)
            {//make sure at least one map is selected
                //creates a mapset for the network manager and starts the game
                MapSet mapSet = new MapSet();
                Debug.Log(mapSet);
                foreach (UI_StageSelectOption stage in selectedStages)
                {
                    mapSet.AddMap(stage.mySceneName);
                }
                NetworkManagerIPS.Instance.SetMapSetAndLeaveLobby(mapSet);
            }
        }


        public void SetTitleText(string title)
        {
            stageTitleDisplay.text = title;
        }
    }
}