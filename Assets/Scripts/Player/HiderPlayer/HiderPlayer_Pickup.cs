using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


namespace IPS.Inputs
{
    public class HiderPlayer_Pickup : NetworkBehaviour
    {
        //Player presses 'Pickup' button when in a certain radius of an Apple to add it to their 'points'
        bool canPickUp;
        Item_Apple currentHighlightedItem;


        private void Update()
        {
            //if spacebar is pressed and there is a value for currentHighlightedItem
            if (Input.GetKeyDown(KeyCode.Space) && canPickUp)
            {
                Pickup();
            }
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Item") && !canPickUp && hasAuthority)
            {
                MatchManagerIPS.Instance.pickupPrompt.gameObject.SetActive(true);
                currentHighlightedItem = other.GetComponent<Item_Apple>();
                canPickUp = true;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Item") && hasAuthority)
            {
                MatchManagerIPS.Instance.pickupPrompt.gameObject.SetActive(false);
                canPickUp = false;
            }
        }


        void Pickup()
        {
            //TODO:use an event to remove the item in the NetworkPlayerSpawnManager
            //MatchManagerIPS.Instance.RemoveItem(currentHighlightedItem);

            //Increase points
            GetComponent<Player_ScoreKeeper>().PlayerCollectItem();
            GUIManager.Instance.DiamondGetDisplay.SetActive(true);
            Destroy(currentHighlightedItem.gameObject);
            canPickUp = false;
        }
    }
}