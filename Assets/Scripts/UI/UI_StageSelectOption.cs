using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Mirror;


namespace IPS.Inputs
{
    public class UI_StageSelectOption : NetworkBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        bool isSelected;
        [SerializeField] Image highlightBox;
        [SerializeField] string stageTitle;
        public string mySceneName;

        [SerializeField] UI_StageSelect stageSelectMain;

        [SerializeField] Sprite mapImage;


        public void OnPointerEnter(PointerEventData eventData)
        {
            stageSelectMain.SetTitleText(stageTitle);
            stageSelectMain.mapImageDisplay.sprite = mapImage;

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            stageSelectMain.SetTitleText("");

        }

        public void SelectStage()
        { //activated on click

            if (!isSelected)
            {
                //add this stage to the selected stages list in UI_StageSelect
                stageSelectMain.selectedStages.Add(this);
                highlightBox.gameObject.SetActive(true);
                isSelected = true;
            }
            else
            {
                highlightBox.gameObject.SetActive(false);
                stageSelectMain.selectedStages.Remove(this);
                isSelected = false;
            }

        }
        public void GoToMyScene()
        {
            NetworkManagerIPS.Instance.ServerChangeScene(mySceneName);
        }
    }
}