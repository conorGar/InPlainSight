using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace IPS.Inputs
{
    public class NetworkLobby : MonoBehaviour
    {
        public static NetworkLobby Instance;
        public Button joinButton;
        public TMP_InputField ipAddressInput;
        public GameObject ipAddressInputUI;

        [SerializeField] public GameObject landingPageHolder;
        [SerializeField] GameObject nameInputPage;

        void Awake()
        {
            Instance = this;
        }

        private void OnEnable()
        {
            NetworkManagerIPS.OnClientConnected += HandleClientConnected;
            NetworkManagerIPS.OnClientConnected += HandleClientDisconnected;

        }

        private void OnDisable()
        {
            NetworkManagerIPS.OnClientConnected -= HandleClientConnected;
            NetworkManagerIPS.OnClientConnected -= HandleClientDisconnected;
        }

        public void HostLobby()
        {
            Debug.Log("?-?-?-? Host Lobby Called?-?_?-?-?");
            Debug.Log(NetworkPlayerSpawnManager.Instance + "< Does the spawn player manager currently exist 2?");

            NetworkManagerIPS.Instance.StartHost();

            landingPageHolder.SetActive(false);
        }


        public void JoinLobby()
        {
            Debug.Log("-x-x-x- JoinLobby called -x-x-x-x");
            NetworkManagerIPS.Instance.networkAddress = ipAddressInput.text;
            NetworkManagerIPS.Instance.StartClient();
            joinButton.interactable = false;
            ipAddressInputUI.SetActive(false);
            landingPageHolder.SetActive(false);

        }

        public void SkipLandingPage()
        { //for use when want to play another match when returning to lobby
            Debug.Log("SKIP LANDING PAGE WAS CALLED IN NETWORK LOBBY!!");
            nameInputPage.SetActive(false);
            landingPageHolder.SetActive(false);
        }


        void HandleClientConnected()
        {
            joinButton.interactable = true;
        }
        void HandleClientDisconnected()
        {
            joinButton.interactable = true;
        }
    }
}