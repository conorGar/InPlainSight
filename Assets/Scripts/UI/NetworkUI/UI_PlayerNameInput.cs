using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PlayerNameInput : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField nameField = null;
    [SerializeField] private Button continueButton = null;

    public static string DisplayName {get; private set;}

    private const string PlayerPrefName = "PlayerName"; //saved player name. I guess steam name would be used here eventually
    
    // Start is called before the first frame update
    void Start()
    {
        nameField.text = PlayerPrefName;

        //turn on/off continue button based on if name is valid. For now valid = not null o
        continueButton.interactable = !string.IsNullOrEmpty(PlayerPrefName);
    }

  
}
